using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для LoadMolecule.xaml
    /// </summary>
    public partial class LoadMolecule : Window
    {
        private Molecule_To_Count _molecule;


        
        public LoadMolecule()
        {
            InitializeComponent();
        }

        private void PasteBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.IDataObject Data = System.Windows.Clipboard.GetDataObject();

            if (Data.GetDataPresent("SMILES"))
            {
                MemoryStream text = Data.GetData("SMILES") as MemoryStream;
                StreamReader sr = new StreamReader(text);
                string Mol = sr.ReadToEnd();
                _molecule = new Molecule_To_Count();
                _molecule.Name = MolName.Text;
                _molecule.LoadFromSMILES(Mol);
                MolImage.Source = _molecule.Image;
            }
        }

        private void MolImage_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, false))
                e.Effects = System.Windows.DragDropEffects.All;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }

        private void MolImage_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
            if (files.Length > 0)
                LoadFromFile(files[0]);
        }

        private void LoadFromFile(string FileName)
        {
            _molecule = new Molecule_To_Count();
            if (_molecule.LoadFromFile(FileName))
                MolImage.Source = _molecule.Image;
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog OD = new OpenFileDialog())
            {
                OD.Filter = "*.cdx|*.cdx";
                OD.Title = "Открыть файл";
                if (OD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    LoadFromFile(OD.FileName);
            }
        }
    }
}
