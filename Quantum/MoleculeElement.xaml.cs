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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для MoleculeElement.xaml
    /// </summary>
    public partial class MoleculeElement : UserControl
    {
        public MoleculeElement()
        {
            InitializeComponent();
            AvailableHeight = ActualHeight - 450;
            SizeChanged += MoleculeElement_SizeChanged;
            Update();
        }

        private void MoleculeElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        protected EnergyElement source;
        protected double min;
        protected double max;
        protected double AvailableHeight;
        protected double Scale;
        protected double ImageText = 200;
        protected bool selected = false;

        /// <summary>
        /// Источник информации о молекуле.
        /// </summary>
        public EnergyElement Source
        {
            get => source;
            set
            {
                source = value;
                HOMO_Image.Source = source.HOMO.Picture;
                LUMO_Image.Source = source.LUMO.Picture;
                Update();
            }
        }

        /// <summary>
        /// Минимальное значение энергии
        /// </summary>
        public double Min
        {
            get => min;
            set
            {
                min = value;
                Update();
            }
        }

        /// <summary>
        /// Максимальное значение энергии
        /// </summary>
        public double Max
        {
            get => max;
            set
            {
                max = value;
                Update();
            }
        }

        public double ImageHeight
        {
            get => ImageText;
            set
            {
                ImageText = value;
                Update();
            }
        }

        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                SelectedRect.Visibility = selected ? Visibility.Visible : Visibility.Hidden;
            }
        }

        

        /// <summary>
        /// Обновить график
        /// </summary>
        public void Update()
        {
            if (Source == null) return;
            if (ActualHeight == 0) return;
            if (ActualWidth == 0) return;

            AvailableHeight = ActualHeight - ImageText * 2 - 
                HOMO_Label.ActualHeight - LUMO_Label.ActualHeight -
                Name_Label.ActualHeight - 20;
            double Delta = Max - Min;
            Scale = Delta / AvailableHeight;

            HOMO_Line.X1 = ActualWidth / 4;
            HOMO_Line.X2 = HOMO_Line.X1 * 3;
            HOMO_Label.Content = Source.HOMO.EnergyString(4);
            HOMO_Label.FontSize = FontSize;
            HOMO_Line.Y1 = (Max - Source.HOMO.Energy) / Scale + ImageText + HOMO_Label.ActualHeight;
            HOMO_Line.Y2 = HOMO_Line.Y1;

            LUMO_Line.X1 = HOMO_Line.X1;
            LUMO_Line.X2 = HOMO_Line.X2;
            LUMO_Label.Content = Source.LUMO.EnergyString(4);
            LUMO_Label.FontSize = FontSize;
            LUMO_Line.Y1 = (Max - Source.LUMO.Energy) / Scale + ImageText + LUMO_Label.ActualHeight;
            LUMO_Line.Y2 = LUMO_Line.Y1;

            
            HOMO_Label.Margin = new Thickness(0, HOMO_Line.Y1, 0, 0);
            LUMO_Label.Margin = new Thickness(0, LUMO_Line.Y1 - LUMO_Label.ActualHeight, 0, 0);
            Delta_Label.Content = Source.GapString(4);
            Delta_Label.FontSize = FontSize * 1.25;
            Delta_Label.Margin = new Thickness(HOMO_Line.X1 + ActualWidth / 10, 
                (HOMO_Line.Y1 - LUMO_Line.Y1 - Delta_Label.ActualHeight)/2 + LUMO_Line.Y1, 0, 0);
            


            Arrow_Line.X1 = HOMO_Line.X1 + ActualWidth / 10;
            Arrow_Line.X2 = Arrow_Line.X1;
            Arrow_Line.Y1 = LUMO_Line.Y1;
            Arrow_Line.Y2 = HOMO_Line.Y1;

            Arrow_Up.Points = new PointCollection()
            {
                new Point(Arrow_Line.X1, LUMO_Line.Y1),
                new Point(Arrow_Line.X1 - ActualWidth / 40, LUMO_Line.Y1 + ActualWidth / 20),
                new Point(Arrow_Line.X1 + ActualWidth / 40, LUMO_Line.Y1 + ActualWidth / 20)
            };

            Arrow_Down.Points = new PointCollection()
            {
                new Point(Arrow_Line.X1, HOMO_Line.Y1),
                new Point(Arrow_Line.X1 - ActualWidth / 40, HOMO_Line.Y1 - ActualWidth / 20),
                new Point(Arrow_Line.X1 + ActualWidth / 40, HOMO_Line.Y1 - ActualWidth / 20)
            };

            HOMO_Image.Margin = new Thickness(0, HOMO_Label.Margin.Top + HOMO_Label.ActualHeight, 0, 0);
            LUMO_Image.Margin = new Thickness(0, LUMO_Label.Margin.Top - LUMO_Image.ActualHeight, 0, 0);
            HOMO_Image.MaxHeight = ImageText;
            LUMO_Image.MaxHeight = ImageText;

            Name_Label.Content = Source.Name;
            Name_Label.FontSize = FontSize;
            Name_Label.Margin = new Thickness(0, HOMO_Image.Margin.Top + HOMO_Image.ActualHeight + 20, 0, 0);

            SelectedRect.Visibility = selected ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
