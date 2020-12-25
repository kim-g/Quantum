using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        string FileName;

        public Energy_Diagram()
        {
            InitializeComponent();
        }

        public static void ShowModal(Window owner)
        {
            using (SaveFileDialog SD = new SaveFileDialog())
            {
                SD.Title = "Сохранить диаграмму";
                SD.DefaultExt = ".edb";
                SD.Filter = "Energy Diagram DataBase files (*.edb)|*.edb|All files (*.*)|*.*";
                SD.CheckFileExists = false;
                SD.OverwritePrompt = false;

                if (SD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                    Energy_Diagram Energy_Diagram_Form = new Energy_Diagram() { Owner = owner };
                    Energy_Diagram_Form.FileName = SD.FileName;
                    Energy_Diagram_Form.Load(Energy_Diagram_Form.FileName);
                    Energy_Diagram_Form.ShowDialog();
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            EnergyElement NewElement = MoleculeEdit.Add();
            if (NewElement != null) Energies.Source.Add(NewElement);
            Save(FileName);
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            Energies.CopyBitmapImageToClipboard();
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            Energies.Print();
        }

        private void Save(string FileName)
        {
            EnergiesDB EDB = EnergiesDB.Open(FileName);
            foreach (EnergyElement EE in Energies.Source.Where(t=>t.Modifyed))
                EDB.SaveMolecule(EE);
        }

        private void Load(string FileName)
        {
            EnergiesDB EDB = EnergiesDB.Open(FileName);
            Energies.Source.Clear();

            using (DataTable dt = EDB.ReadTable("SELECT `id` FROM `elements`"))
            {
                foreach (DataRow dr in dt.Rows)
                    Energies.Source.Add(EDB.LoadMolecule(Convert.ToInt64(dr["id"])));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Save(FileName);
        }

        private void Energies_Updated(object sender)
        {
            Save(FileName);
        }
    }
}
