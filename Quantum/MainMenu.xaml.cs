﻿using System.Windows;

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
        public static string ProgramVersion = "1.5.3";
        /// <summary>
        /// База данных конфига
        /// </summary>
        SQLiteConfig Config;

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
            Config = SQLiteConfig.Open("config.db");
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
            Energy_Diagram.ShowModal(this);
        }

        private void ConstructorButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectListWindow PLW = new ProjectListWindow(Config);
            PLW.ShowDialog();
            
            
        }
    }
}
