using Microsoft.Win32;
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
    /// Логика взаимодействия для PeptideSetup.xaml
    /// </summary>
    public partial class PeptideSetup : Window
    {
        public PeptideSetup()
        {
            InitializeComponent();
        }

        public static PeptideSetup Add()
        {
            return new PeptideSetup();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string FilePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDBQT файлы|*.pdbqt";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;

                return;
            }
        }
    }
}
