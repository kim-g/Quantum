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
    /// Логика взаимодействия для Energy_Diagram.xaml
    /// </summary>
    public partial class Energy_Diagram : Window
    {
        SQLiteDataBase Config;
        EnergyElement EE;
        int N = 0;

        public Energy_Diagram(SQLiteDataBase ConfigDataBase)
        {
            InitializeComponent();
            Config = ConfigDataBase;
        }

        public static void ShowModal(Window owner, SQLiteDataBase ConfigDataBase)
        {
            Energy_Diagram Energy_Diagram_Form = new Energy_Diagram(ConfigDataBase)
            {
                Owner = owner
            };
            Energy_Diagram_Form.ShowDialog();
            GC.Collect();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            EnergyElement NewElement = MoleculeEdit.Add();
            if (NewElement != null) Energies.Source.Add(NewElement);
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            Energies.CopyBitmapImageToClipboard();

        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            Energies.Print();
        }
    }
}
