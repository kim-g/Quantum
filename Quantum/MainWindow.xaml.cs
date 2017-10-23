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


namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                    File.WriteAllText(System.IO.Path.Combine(CurAddress, TemplateNames[j]),
                        Templates[j].Replace("@Dir@", CurDirLocalUnix).Replace("@N@", CurDir));
                }
              
            }

            MessageBox.Show("Файлы созданы успешно", "Работа завершена");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
