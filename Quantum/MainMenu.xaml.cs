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
        public static string ProgramVersion = "1.2.0";
        SQLiteDataBase Config;

        public MainMenu()
        {
            InitializeComponent();
            Config = SQLiteDataBase.Open("config.db");
        }

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Projects = new MainWindow(Config);
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

        private void MOButton_Click(object sender, RoutedEventArgs e)
        {
            SetMO SetMO_Form = new SetMO(Config);
            SetMO_Form.Owner = this;
            SetMO_Form.ShowDialog();
            GC.Collect();
        }
    }
}
