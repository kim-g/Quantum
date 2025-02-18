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
    /// Логика взаимодействия для AutoDockCreator.xaml
    /// </summary>
    public partial class AutoDockCreator : Window
    {
        public AutoDockCreator()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            if (GenRangeSP == null) return;
            GenRangeSP.Visibility = Visibility.Visible;
            GenListGrid.Visibility = Visibility.Collapsed;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (GenRangeSP == null) return;
            GenRangeSP.Visibility = Visibility.Collapsed;
            GenListGrid.Visibility = Visibility.Visible;
        }

        private void NumbersTB_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void AddCountBtn_Click(object sender, RoutedEventArgs e)
        {
            string Task = AddParam.AddString("Добавить расчётное задание");
            if (Task == null) return;
            CountList.Items.Add(Task);
        }

        private void DeleteCountBtn_Click(object sender, RoutedEventArgs e)
        {
            CountList.Items.Remove(CountList.SelectedItem);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            PeptideSetup PS = PeptideSetup.Add();
            PS.ShowDialog();
        }
    }
}
