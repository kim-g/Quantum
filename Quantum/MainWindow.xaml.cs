using Extentions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteConfig Config;
        Dictionary<string, string> Roots;
        Dictionary<string, string> RootLCTN;
        Dictionary<string, string> Solvents;

        public MainWindow(SQLiteConfig ConfigDataBase)
        {
            InitializeComponent();
            Config = ConfigDataBase;

            Dir_text.Text = Config.GetConfigValue("work_dir");
            Project_text.Text = Config.GetConfigValue("project_dir");
            from_text.Text = Config.GetConfigValue("from");
            to_text.Text = Config.GetConfigValue("to");
            Additional.Text = Config.GetConfigValue("additional");
            List_text.Text = Config.GetConfigValue("list_text");
            ChargeTB.Text = "0";
            MultiTB.Text = "1";
            Atom1.Text = Config.GetConfigValue("ConstBondAtom1");
            Atom2.Text = Config.GetConfigValue("ConstBondAtom2");
            BondLength.Text = Config.GetConfigValue("ConstBondLength");
            Charges.IsChecked = Config.GetConfigValueBool("Charges");
            Hessian.IsChecked = Config.GetConfigValueBool("numfreq");

            List<string> Templates = Directory.EnumerateDirectories("Templates").ToList();
            foreach (string Template in Templates)
            {
                string[] DirAddress = Template.Split('\\');
                TemplateDir.Items.Add(DirAddress[1]);
                if (DirAddress[1] == Config.GetConfigValue("template_dir"))
                    TemplateDir.SelectedIndex = TemplateDir.Items.Count - 1;
            }

            Roots = new Dictionary<string, string>();
            RootLCTN = new Dictionary<string, string>();
            using (DataTable dt = Config.ReadTable("SELECT * FROM `roots`"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string name = dr["name"].ToString();
                    Roots.Add(name, dr["path"].ToString());
                    RootLCTN.Add(name, dr["lctndir"].ToString());
                    RootCB.Items.Add(dr["name"].ToString());
                    string root = Config.GetConfigValue("root");
                    if (root == name)
                        RootCB.SelectedIndex = RootCB.Items.Count - 1;
                }
            }

            Solvents = new Dictionary<string, string>();
            using (DataTable dt = Config.ReadTable("SELECT * FROM `solvents` WHERE `id`>0"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string name = dr["name"].ToString();
                    Solvents.Add(name, dr["code"].ToString());
                    SolventCB.Items.Add(dr["name"].ToString());
                    if (Config.GetConfigValue("solvent") == name)
                        SolventCB.SelectedIndex = SolventCB.Items.Count - 1;
                }
            }

            RB_Diap.IsChecked = !Config.GetConfigValueBool("list");
            RB_List.IsChecked = Config.GetConfigValueBool("list");
        }

        /// <summary>
        /// Показывает окно создания расчётных задач
        /// </summary>
        /// <param name="owner">Окно-родитель</param>
        /// <param name="ConfigDataBase">База данных конфигурации</param>
        public static void ShowModal(Window owner, SQLiteConfig ConfigDataBase)
        {
            MainWindow Projects = new MainWindow(ConfigDataBase);
            Projects.Owner = owner;
            Projects.ShowDialog();
            GC.Collect();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Ищем все темплаты
            string[] allTemplates = Directory.GetFiles("Templates\\" + TemplateDir.Text, "*", 
                SearchOption.TopDirectoryOnly);
            List<string> Templates = new List<string>();
            List<string> TemplateNames = new List<string>();
            foreach (string file in allTemplates)
            {
                Templates.Add(File.ReadAllText(file));
                TemplateNames.Add(System.IO.Path.GetFileName(file));
            }

            // получаем рабочую директорию
            string WorkDirectory = System.IO.Path.Combine(Dir_text.Text, Project_text.Text);

            if (RB_Diap.IsChecked == true)
            {
                // Получаем значения от и до
                int From;
                int To;
                try
                { From = Convert.ToInt32(from_text.Text); }
                catch
                { MessageBox.Show("Введите число в поле От", "Ошибка"); return; }
                try
                { To = Convert.ToInt32(to_text.Text); }
                catch
                { MessageBox.Show("Введите число в поле До", "Ошибка"); return; }

                // И проходим по всем внутренним директориям
                for (int i = From; i <= To; i++)
                {
                    CreateProject(Templates, TemplateNames, WorkDirectory, i.ToString("D2"));
                }
            }
            else
            {
                // Получаем список
                string[] DirList = List_text.Text.Split(';');

                // И проходим его
                foreach (string Dir in DirList)
                {
                    string CurDir = Dir.Trim();
                    if (String.IsNullOrWhiteSpace(CurDir)) continue;
                    CreateProject(Templates, TemplateNames, WorkDirectory, CurDir);
                }
            }

            MessageBox.Show("Файлы созданы успешно", "Работа завершена");
        }

        private void CreateProject(List<string> Templates, List<string> TemplateNames, string WorkDirectory, string CurDir)
        {
            T SetItem<T>(string Text, string Target)
            {
                T IntValue = default(T);
                Char separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
                string NewText = Text.Trim().Replace('.', separator).Replace(',',separator).Replace(" ", "");
                
                if (NewText == "")
                {
                    MessageBox.Show($"Введите {Target}!");
                    return default(T);
                }
                try
                {
                    IntValue = NewText.To<T>();
                }
                catch
                {
                    MessageBox.Show($"Неправильно введён {Target}!");
                }
                return IntValue;
            }
            
            // Формируем название
            string CurAddress = System.IO.Path.Combine(WorkDirectory, CurDir);
            string CurDirLocalUnix = System.IO.Path.Combine(Project_text.Text, CurDir).Replace('\\', '/');

            // Создаём соответствующую директорию
            Directory.CreateDirectory(CurAddress);

            // И записывает туда изменённые файлы
            for (int j = 0; j < Templates.Count; j++)
            {
                string TextOut = Templates[j].Replace("@Dir@", CurDirLocalUnix).Replace("@N@", CurDir).
                    Replace("@Charge@", ChargeTB.Text).Replace("@Mult@", MultiTB.Text).
                    Replace("@root@", Roots[RootCB.SelectedItem.ToString()]).
                    Replace("@Hessian@", (bool)Hessian.IsChecked ? "numfreq" : "freq").
                    Replace("@lctndir@", RootLCTN[RootCB.SelectedItem.ToString()]);

                // Получаем дополнительные параметры
                string More = "";

                if (Charges.IsChecked == true)
                    More +="#Вывести зарядовые плотности\n%output\n\tPrint[P_AtPopMO_M] 1\nend\n";

                if (ConstBond.IsChecked == true)
                {
                    int First, Second;
                    float Bond;
                    First = SetItem<int>(Atom1.Text, "номер первого атома");
                    Second = SetItem<int>(Atom2.Text, "номер второго атома");
                    Bond = SetItem<float>(BondLength.Text, "длину связи");

                    string BondStr = Bond.ToString().
                        Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0], '.');
                    More += "#Зафиксировать связь\n%geom \n    constraints { B " + $"{First} {Second} {BondStr} " + " C } end \nend\n";
                }

                if (SolventCB.SelectedIndex > 0)
                {
                    More += $"!CPCMC({Solvents[SolventCB.SelectedItem.ToString()]})     # Растворитель: {SolventCB.SelectedItem}\n";
                }

                TextOut = TextOut.Replace("@More@", More);

                string[] Add_Rep = Additional.Text.Split(';');
                foreach (string Rep in Add_Rep)
                {
                    string[] ReplaceText = Rep.Trim().Split('=');
                    if (ReplaceText.Length > 1)
                        TextOut = TextOut.Replace(ReplaceText[0], ReplaceText[1]);
                }
                File.WriteAllText(System.IO.Path.Combine(CurAddress, TemplateNames[j]), TextOut);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string Folder = Get_Directory(Dir_text.Text);
            if (Folder != null)
            {
                Dir_text.Text = Folder;
            }
        }


        /// <summary>
        /// Показывает диалоговое окно выбора директории
        /// </summary>
        private string Get_Directory(string Selected = "")
        {
            WinForms.FolderBrowserDialog folderBrowser = new WinForms.FolderBrowserDialog();

            folderBrowser.SelectedPath = Selected;

            WinForms.DialogResult result = folderBrowser.ShowDialog();

            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                return folderBrowser.SelectedPath;
            }
            return null;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            string Old = Project_text.Text == "" ? Dir_text.Text : Dir_text.Text + "\\" + Project_text.Text;
            string Folder = Get_Directory(Old);
            if (Folder != null)
            {
                Project_text.Text = Folder.Replace(Dir_text.Text, "").TrimStart('\\');
            }
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            string Folder = Get_Directory(TemplateDir.Text);
            if (Folder != null)
            {
                TemplateDir.Text = Folder;
            }
        }

        private void Dir_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue("work_dir", Dir_text.Text);
        }

        private void Project_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue("project_dir", Project_text.Text);
        }

        private void TemplateDir_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue("template_dir", TemplateDir.Text);
        }

        private void from_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue("from", from_text.Text);
        }

        private void to_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue("to", to_text.Text);
        }

        private void Additional_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue("additional", Additional.Text);
        }

        private void RB_Diap_Checked(object sender, RoutedEventArgs e)
        {
            Diap.Visibility = Visibility.Visible;
            List_Grid.Visibility = Visibility.Collapsed;
            if (Config != null) Config.SetConfigValue("list", false);
            Form1.SizeToContent = SizeToContent.Height;
        }

        private void RB_List_Checked(object sender, RoutedEventArgs e)
        {
            Diap.Visibility = Visibility.Collapsed;
            List_Grid.Visibility = Visibility.Visible;
            if (Config != null) Config.SetConfigValue("list", true);
            Form1.SizeToContent = SizeToContent.Height;
        }

        private void List_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue("list_text", List_text.Text);
        }

        private void TemplateDir_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.SetConfigValue("template_dir", TemplateDir.SelectedItem.ToString());
        }

        private void ConstNone_Checked(object sender, RoutedEventArgs e)
        {
            if (ConstBondStack == null) return;
            ConstBondStack.Visibility = Visibility.Collapsed;
        }

        private void ConstBond_Checked(object sender, RoutedEventArgs e)
        {
            if (ConstBondStack == null) return;
            ConstBondStack.Visibility = Visibility.Visible;
        }

        private void Charges_Click(object sender, RoutedEventArgs e)
        {
            Config.SetConfigValue("Charges", Charges.IsChecked == true);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Config.SetConfigValue(((TextBox)sender).Tag.ToString(), ((TextBox)sender).Text);
        }

        private void RootCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.SetConfigValue("root", RootCB.SelectedItem.ToString());
        }

        private void SolventCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.SetConfigValue("solvent", SolventCB.SelectedItem.ToString());
            if (SolventCB.SelectedIndex > 0)
            {
                Hessian.IsChecked = true;
                Hessian.IsEnabled = false;
            }
            else
            {
                Hessian.IsChecked = Config.GetConfigValueBool("numfreq");
                Hessian.IsEnabled = true;
            }
        }

        private void Hessian_Click(object sender, RoutedEventArgs e)
        {
            Config.SetConfigValue("numfreq", Hessian.IsChecked == true);
        }
    }
}
