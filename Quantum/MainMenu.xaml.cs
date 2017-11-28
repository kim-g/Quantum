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
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public static string ProgramVersion = "1.1.0";

        public MainMenu()
        {
            InitializeComponent();
        }

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Projects = new MainWindow();
            Projects.ShowDialog();
            GC.Collect();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About AboutForm = new About();
            AboutForm.ShowDialog();
            GC.Collect();
        }
    }
}
