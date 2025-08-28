using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для Energy_Diagram.xaml
    /// </summary>
    public partial class Energy_Diagram : Window
    {
        string FileName;
        EnergiesDB DB;
        private delegate void SimpleDelegate();


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
                    Energy_Diagram Energy_Diagram_Form = new Energy_Diagram
                    {
                        Owner = owner,
                        FileName = SD.FileName
                    };
                    Energy_Diagram_Form.Load();
                    Energy_Diagram_Form.ShowDialog();
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            EnergyElement NewElement = MoleculeEdit.Add();
            if (NewElement != null) Energies.Source.Add(NewElement);
            Save();
        }

        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            Energies.CopyBitmapImageToClipboard();
        }

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            Energies.Print(this);
        }

        private void Save()
        {
            if (Energies == null) return;
            if (DB == null) return;
            
            DB.SaveParameter("fontsize", Energies.FontSize);

            foreach (EnergyElement EE in Energies.Source.Where(t=>t.Modifyed))
                DB.SaveMolecule(EE);
        }

        private void Load()
        {
            DB = EnergiesDB.Open(FileName);

            string FS = DB.LoadParameter("fontsize");
            if (FS == null)
                DB.SaveParameter("fontsize", 20);
            else
                Energies.FontSize = double.Parse(FS);

            FontSizeSlider.Value = Energies.FontSize;

            Energies.Source.Clear();
            using (DataTable dt = DB.ReadTable("SELECT `id` FROM `elements`"))
            {
                foreach (DataRow dr in dt.Rows)
                    Energies.Source.Add(DB.LoadMolecule(Convert.ToInt64(dr["id"])));
            }
        }

        private void FontSize_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Console.WriteLine("FontSize changing stop");
        }

        private void FontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Energies.FontSize = FontSizeSlider.Value;
            FS_Label.Content = $"Размер шрифта: {FontSizeSlider.Value:F1}";

            Energies.Update();
            Dispatcher.BeginInvoke(new SimpleDelegate(Save), null) ;
        }
    }
}
