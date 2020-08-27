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
        EnergyElement EE = new EnergyElement();

        public Energy_Diagram(SQLiteDataBase ConfigDataBase)
        {
            InitializeComponent();
            Config = ConfigDataBase;

            EE.HOMO.Energy = -2.3456;
            EE.LUMO.Energy = -5.6789;
            EE.Name = "Test";

            Test.Source = EE;
            Test.Min = -6;
            Test.Max = -2;
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

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            char Separatop = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];


            EE.HOMO.Energy = Convert.ToDouble(MinVal.Text.Replace('.', Separatop).Replace(',',Separatop));
            EE.LUMO.Energy = Convert.ToDouble(MaxVal.Text.Replace('.', Separatop).Replace(',', Separatop));
            Test.Update();
        }
    }
}
