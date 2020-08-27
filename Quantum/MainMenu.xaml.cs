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
        /// <summary>
        /// Версия программы
        /// </summary>
        public static string ProgramVersion = "1.4.0";
        /// <summary>
        /// База данных конфига
        /// </summary>
        SQLiteDataBase Config;

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
            Config = SQLiteDataBase.Open("config.db");
        }

        /// <summary>
        /// Нажатие на кнопку "Проекты расчётов из шаблонов"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ShowModal(this, Config);
        }

        /// <summary>
        /// Нажатие кнопки "Закрыть"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Нажатие кнопки "О программе"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About.ShowModal(this);
        }

        /// <summary>
        /// Показать окно редактирования расчёта МО (Устарело)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MOButton_Click(object sender, RoutedEventArgs e)
        {
            SetMO.ShowModal(this, Config);
        }

        private void EnergyButton_Click(object sender, RoutedEventArgs e)
        {
            Energy_Diagram.ShowModal(this, Config);
        }
    }
}
