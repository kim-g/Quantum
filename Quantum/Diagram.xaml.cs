using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
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

        public delegate void UpdateEvent(object sender);
        public event UpdateEvent Updated;

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
                    ME.MouseDoubleClick += OnDblClick;
                    ME.MouseLeftButtonDown += ME_MouseLeftButtonDown;
                    Field.Children.Add(ME);
                    Elements.Add(ME);
                }
            }
            if (Diff < 0)
            {
                for (int i = 0; i > Diff; i--)
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

            Updated?.Invoke(this);
        }

        private void ME_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MoleculeElement ME = (MoleculeElement)sender;
            ME.Selected = !ME.Selected;
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
            WPF_to_WMF_Converter.CopyUIElementToClipboard(Field, 3);
        }

        /// <summary>
        /// Печать диаграммы
        /// </summary>
        public void Print(Window Owner)
        {
            /*PrintDialog printDlg = new PrintDialog();
            printDlg.PrintTicket.PageMediaSize
                    = new PageMediaSize(Field.ActualWidth, Field.ActualHeight);
            PrintTicket prntkt = printDlg.PrintTicket;
            prntkt.PageMediaSize = new PageMediaSize(Field.ActualWidth, Field.ActualHeight);
            prntkt.PageOrientation = PageOrientation.Landscape;
            if (/*printDlg.ShowDialog() == true)*/
            /*{
                prntkt = printDlg.PrintTicket;
                prntkt.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA0);
                prntkt.PageOrientation = PageOrientation.Landscape;
                printDlg.PrintTicket = prntkt;
                printDlg.PrintVisual(Field, "Диаграмма энергий.");
                //PrintHelper.ShowPrintPreview(PrintHelper.GetFixedDocument(Field, printDlg));
            }*/

                WPF_to_WMF_Converter.SaveUIElementToXAML(Field);
        }

        private void OnDblClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MoleculeElement ME = (MoleculeElement)sender;
                ME.Source = MoleculeEdit.Edit(ME.Source);
                Update();
            }
        }

    }
}
