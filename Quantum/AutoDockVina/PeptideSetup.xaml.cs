using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Логика взаимодействия для PeptideSetup.xaml
    /// </summary>
    public partial class PeptideSetup : Window
    {
        private bool adding = false;    // Добавление ли это
        private bool added = false;     // Добавлено ли
        string FilePath = "";           // Путь к файлу пептида

        public PeptideSetup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавление пептида
        /// </summary>
        /// <returns>Был ли пептид добавлен в БД</returns>
        public static bool Add()
        {
            PeptideSetup PS = new PeptideSetup();
            PS.adding = true;
            PS.OK_BTN.Content = "Добавить";
            PS.ShowDialog();
            return PS.added;
        }

        /// <summary>
        /// Загрузка файла пептида
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDBQT файлы|*.pdbqt";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                FileName.Content = System.IO.Path.GetFileName(FilePath) ;
                return;
            }
        }

        /// <summary>
        /// Добавить пептид в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (adding)
            {
                string CenterX = ToFloatString(CenterXTB.Text, 3, "X координаты центра");
                string CenterY = ToFloatString(CenterYTB.Text, 3, "Y координаты центра");
                string CenterZ = ToFloatString(CenterZTB.Text, 3, "Z координаты центра");
                string SizeX = ToFloatString(SizeXTB.Text, 1, "X размера");
                string SizeY = ToFloatString(SizeYTB.Text, 1, "Y размера");
                string SizeZ = ToFloatString(SizeZTB.Text, 1, "Z размера");
                using (FileStream file = new FileStream(FilePath, FileMode.Open))
                {
                    MainMenu.Config.ExecuteBLOB("INSERT INTO `proteins` (`name`, `description`, `file_name`, `center_x`, `center_y`, `center_z`, " +
                        "`size_x`, `size_y`, `size_z`, `file`) VALUES ('" + PeptidNameTB.Text + "', '" + PeptidDescriptionTB.Text + "', '" + FileName.Content + "', '" +
                        CenterX + "', '" + CenterY + "', '" + CenterZ + "', '" + SizeX + "', '" + SizeY + "', '" + SizeZ + "', @BLOB)", file);
                    file.Close();
                }
                added = true;

                Close();
            }
        }

        private string ToFloatString(string s, int Digits, string Error)
        {
            try
            {
                char separator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
                s = s.Trim().Replace('.', separator).Replace(',', separator);
                double f = Convert.ToDouble(s);
                return f.ToString($"F{Digits}").Replace(separator, '.');
            }
            catch
            {
                throw new Exception($"Неверный формат {Error}");
            }
        }
    }
}
