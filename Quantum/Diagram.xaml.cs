using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для Diagram.xaml
    /// </summary>
    public partial class Diagram : UserControl
    {
        protected Brush diagrambackground;
        protected Brush legendbackground;
        protected ObservableCollection<EnergyElement> source = new ObservableCollection<EnergyElement>();
        protected int SourceElementsOld = 0;
        protected List<MoleculeElement> Elements = new List<MoleculeElement>();
        protected double MinEnergy = -1;
        protected double MaxEnergy = 0;
        protected double elementwidth = 200;
        
        /// <summary>
        /// Цвет фона основного пространства диаграммы
        /// </summary>
        public Brush DiagramBackground
        {
            get => diagrambackground;
            set
            {
                diagrambackground = value;
                Field.Background = diagrambackground;
            }
        }

        /// <summary>
        /// Цвет фона легенды диаграммы
        /// </summary>
        public Brush LegendBackground
        {
            get => legendbackground;
            set
            {
                legendbackground = value;
                Legend.Background = legendbackground;
            }
        }

        /// <summary>
        /// Список соединений
        /// </summary>
        public ObservableCollection<EnergyElement> Source
        {
            get => source;
            set
            {
                source = value;
            }
        }

        public double ElementWidth
        {
            get => elementwidth;
            set
            {
                elementwidth = value;
                Update();
            }
        }

        public Diagram()
        {
            InitializeComponent();
            source.CollectionChanged += Source_CollectionChanged;
        }

        private void Source_CollectionChanged(object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Обновляет диаграмму
        /// </summary>
        public void Update()
        {
            int Diff = Source.Count - SourceElementsOld;
            if (Diff > 0)
            {
                for (int i = 0; i < Diff; i++)
                {
                    MoleculeElement ME = new MoleculeElement();
                    Field.Children.Add(ME);
                    Elements.Add(ME);
                }
            }
            if (Diff < 0)
            {
                for (int i = 0; i < Diff; i++)
                {
                    MoleculeElement ME = Elements[Elements.Count - 1];
                    Field.Children.Remove(ME);
                    Elements.Remove(ME);
                }
            }
            SourceElementsOld = Source.Count;

            SetMinMax();
            int N = 0;
            foreach (EnergyElement EE in Source)
            {
                Elements[N].Source = EE;
                Elements[N].Min = MinEnergy;
                Elements[N].Max = MaxEnergy;
                Elements[N].FontSize = FontSize;
                Elements[N].Update();

                N++;
            }
        }

        /// <summary>
        /// Определение минимального и минимального значения энергии в массиве
        /// </summary>
        protected void SetMinMax()
        {
            if (Source.Count == 0) return;
            MinEnergy = Source[0].HOMO.Energy;
            MaxEnergy = Source[0].LUMO.Energy;

            foreach (EnergyElement EE in Source)
            {
                if (EE.HOMO.Energy < MinEnergy) MinEnergy = EE.HOMO.Energy;
                if (EE.LUMO.Energy > MaxEnergy) MaxEnergy = EE.LUMO.Energy;
            }
        }

        /// <summary>
        /// Копирует готовую таблицу в буфер обмена
        /// </summary>
        public void CopyBitmapImageToClipboard()
        {
            WPF_to_WMF_Converter.CopyUIElementToClipboard(Field);
        }

        /// <summary>
        /// Печать диаграммы
        /// </summary>
        public void Print()
        {
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
                printDlg.PrintVisual(Field, "Диаграмма энергий.");
        }
    }
}
