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

namespace Quantum.AutoDockVina
{
    /// <summary>
    /// Логика взаимодействия для ProteinBase.xaml
    /// </summary>
    public partial class ProteinBase : Window
    {
        public List<Protein> Selected
        {
            get => ProteinListBox.SelectedItems.Cast<Protein>().ToList();
        }
        
        public ProteinBase()
        {
            InitializeComponent();
        }

        public static List<Protein> SelectProteins()
        {
            List<Protein> PS = Protein.LoadProteins();
            ProteinBase PB = new ProteinBase();
            PB.ProteinListBox.SelectionMode = SelectionMode.Multiple;
            PB.ProteinListBox.ItemsSource = PS;
            if (PB.ShowDialog() == true)
                return PB.Selected;
            else return null;
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterPeptides(SearchTextBox.Text);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddPeptide_Click(object sender, RoutedEventArgs e)
        {
            if (PeptideSetup.Add())
            {
                List<Protein> PS = Protein.LoadProteins();
                ProteinListBox.ItemsSource = PS;
            }
        }

        private void FilterPeptides(string filterText)
        {
            // Получаем представление коллекции для фильтрации
            var view = CollectionViewSource.GetDefaultView(ProteinListBox.ItemsSource);

            if (view == null) return;

            // Устанавливаем фильтр
            view.Filter = item =>
            {
                // Если фильтр пустой - показываем все элементы
                if (string.IsNullOrWhiteSpace(filterText)) return true;

                // Приводим элемент к Protein и проверяем наличие подстроки
                var peptide = item as Protein;
                bool Filtered = peptide?.Name.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
                Filtered = Filtered || peptide?.Description.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
                return Filtered;
            };
        }
    }
}
