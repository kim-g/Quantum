using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для RunWindow.xaml
    /// </summary>
    public partial class RunWindow : Window
    {
        public RunWindow()
        {
            InitializeComponent();
        }

        public static RunData NewRun()
        {
            RunWindow RW = new RunWindow();
            if (RW.ShowDialog() == true)
                return new RunData() { Text = RW.TaskName.Text, Parallel = RW.ParralelChB.IsChecked == true };
            return new RunData();
        }

        public static RunData EditRun(RunData rd)
        {
            RunWindow RW = new RunWindow();
            RW.TaskName.Text = rd.Text;
            RW.ParralelChB.IsChecked = rd.Parallel;
            RW.Title = "Редактировать файл запуска";
            RW.AddBtn.Content = "Редактировать";
            if (RW.ShowDialog() == true)
                return new RunData() { Text = RW.TaskName.Text, Parallel = RW.ParralelChB.IsChecked == true };
            return new RunData();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public struct RunData
    {
        public string Text;
        public bool Parallel;
    }
}
