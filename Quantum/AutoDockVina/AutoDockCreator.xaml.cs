using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Логика взаимодействия для AutoDockCreator.xaml.
    /// Окно для создания задач AutoDock с выбором белков и лигандов.
    /// </summary>
    public partial class AutoDockCreator : Window
    {
        /// <summary>
        /// Модель представления для управления списком белков, лигандов и связанных данных.
        /// </summary>
        CreatorModel model = new CreatorModel();

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AutoDockCreator"/>.
        /// </summary>
        public AutoDockCreator()
        {
            InitializeComponent();
            DataContext = model;
        }

        /// <summary>
        /// Обработчик события для проверки ввода только чисел.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void NumbersTB_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        /// <summary>
        /// Обработчик события для добавления нового лиганда в список.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void AddCountBtn_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалог выбора файлов
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Chemical Files|*.mol;*.sdf;*.cdx;*.cdxml;*.pdb;*.pdbqt;*.xyz;*.cif;*.mol2;*.smiles;*.bv|" +
                         "2D Formats (*.mol, *.sdf, *.cdx, *.cdxml, *.smiles)|*.mol;*.sdf;*.cdx;*.cdxml;*.smiles|" +
                         "3D Formats (*.pdb, *.pdbqt, *.xyz, *.cif, *.mol2)|*.pdb;*.pdbqt;*.xyz;*.cif;*.mol2|" +
                         "BV Files (*.bv)|*.bv",
                Title = "Выберите лиганд"
            };

            // Показываем диалог и проверяем результат
            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем существующие пути для проверки дубликатов
                var existingPaths = model.LigandFileList.Cast<Ligand>()
                                        .Select(f => f.FileName)
                                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // Добавляем новые файлы
                foreach (var filePath in openFileDialog.FileNames)
                {
                    if (!existingPaths.Contains(filePath))
                    {
                        model.LigandFileList.Add(new Ligand(filePath));
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик события для удаления выбранного лиганда из списка.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void DeleteCountBtn_Click(object sender, RoutedEventArgs e)
        {
            model.LigandFileList.Remove(model.LigandFileSelected);
        }

        /// <summary>
        /// Обработчик события для добавления белков в список.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            List<Protein> LP = ProteinBase.SelectProteins();
            if (LP == null) return;
            foreach (Protein P in LP)
                model.ProteinList.Add(P);
        }

        /// <summary>
        /// Обработчик события для удаления выбранного белка из списка.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            model.ProteinList.Remove(model.SelectedProtein);
        }

        /// <summary>
        /// Обработчик события для генерации списка задач.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void GenListBtn_Click(object sender, RoutedEventArgs e)
        {
            Progress<string> progress = new Progress<string>(status =>
            {
                model.Log(status); // Безопасное обновление Label
            });
            Task.Run(() => model.CreateProject(progress));
        }

        /// <summary>
        /// Обработчик события для изменения выбранного пользователя.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void OrdererCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainMenu.Config.SetConfigValue("autodock_orderer", model.UserSelected);
        }

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = model.RunTask();
        }

        private void AnalyseBtn_Click(object sender, RoutedEventArgs e)
        {
            var progress = new Progress<string>(status =>
            {
                model.Log(status);
            });
            _ = model.Analyse(progress);
        }
    }
}
