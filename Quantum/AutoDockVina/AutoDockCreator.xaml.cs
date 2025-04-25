using System.Windows;

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Логика взаимодействия для AutoDockCreator.xaml
    /// </summary>
    public partial class AutoDockCreator : Window
    {
        /// <summary>
        /// Модель представления для управления списком белков и выбранным белком.
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
        /// Обработчик события для отображения диапазона генерации.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            if (GenRangeSP == null) return;
            GenRangeSP.Visibility = Visibility.Visible;
            GenListGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Обработчик события для отображения списка генерации.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (GenRangeSP == null) return;
            GenRangeSP.Visibility = Visibility.Collapsed;
            GenListGrid.Visibility = Visibility.Visible;
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
            string Task = AddParam.AddString("Добавить расчётное задание");
            if (Task == null) return;
            CountList.Items.Add(Task);
        }

        /// <summary>
        /// Обработчик события для удаления выбранного лиганда из списка.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void DeleteCountBtn_Click(object sender, RoutedEventArgs e)
        {
            CountList.Items.Remove(CountList.SelectedItem);
        }

        /// <summary>
        /// Обработчик события для добавления белков в список.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (Protein P in ProteinBase.SelectProteins())
                model.proteinList.Add(P);
        }

        /// <summary>
        /// Обработчик события для удаления выбранного белка из списка.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные события.</param>
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            model.proteinList.Remove(ProteinList.SelectedItem as Protein);
        }
    }
}
