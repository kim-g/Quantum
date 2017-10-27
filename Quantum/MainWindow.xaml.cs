using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteDataBase Config;

        public MainWindow()
        {
            InitializeComponent();
            Config = SQLiteDataBase.Open("config.db");

            Dir_text.Text = Config.GetConfigValue("work_dir");
            Project_text.Text = Config.GetConfigValue("project_dir");
            TemplateDir.Text = Config.GetConfigValue("template_dir");
            from_text.Text = Config.GetConfigValue("from");
            to_text.Text = Config.GetConfigValue("to");
            Additional.Text = Config.GetConfigValue("additional");
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Ищем все темплаты
            string[] allTemplates = Directory.GetFiles(TemplateDir.Text, "*", SearchOption.TopDirectoryOnly);
            List<string> Templates = new List<string>();
            List<string> TemplateNames = new List<string>();
            foreach (string file in allTemplates)
            {
                Templates.Add(File.ReadAllText(file));
                TemplateNames.Add(System.IO.Path.GetFileName(file));
            }

            // Получаем значения от и до
            int From;
            int To;
            try
            { From = Convert.ToInt32(from_text.Text); }
            catch
            { MessageBox.Show("Введите число в поле От","Ошибка"); return; }
            try
            { To = Convert.ToInt32(to_text.Text); }
            catch
            { MessageBox.Show("Введите число в поле До", "Ошибка"); return; }

            // получаем рабочую директорию
            string WorkDirectory = System.IO.Path.Combine(Dir_text.Text, Project_text.Text);

            // И проходим по всем внутренним директориям
            for (int i= From; i <= To; i++)
            {
                // Формируем название
                string CurDir = i.ToString("D2");
                string CurAddress = System.IO.Path.Combine(WorkDirectory, CurDir);
                string CurDirLocalUnix = System.IO.Path.Combine(Project_text.Text, CurDir).Replace('\\','/');

                // Создаём соответствующую директорию
                Directory.CreateDirectory(CurAddress);

                // И записывает туда изменённые файлы
                for (int j=0; j<Templates.Count; j++)
                {
                    string TextOut = Templates[j].Replace("@Dir@", CurDirLocalUnix).Replace("@N@", CurDir);
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

            MessageBox.Show("Файлы созданы успешно", "Работа завершена");
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
    }
}
