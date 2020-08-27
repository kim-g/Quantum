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
        }

        protected EnergyElement source;
        protected double min;
        protected double max;
        protected double AvailableHeight;
        protected double Scale;
        protected double ImageText = 240;

        /// <summary>
        /// Источник информации о молекуле.
        /// </summary>
        public EnergyElement Source
        {
            get => source;
            set
            {
                source = value;
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

        /// <summary>
        /// Обновить график
        /// </summary>
        public void Update()
        {
            if (Source == null) return;
            if (ActualHeight == 0) return;

            AvailableHeight = ActualHeight - ImageText * 2;
            double Delta = Max - Min;
            Scale = Delta / AvailableHeight;


            HOMO_Line.Y1 = (Max - Source.HOMO.Energy) / Scale + ImageText;
            HOMO_Line.Y2 = HOMO_Line.Y1;

            LUMO_Line.Y1 = (Max - Source.LUMO.Energy) / Scale + ImageText;
            LUMO_Line.Y2 = LUMO_Line.Y1;

            HOMO_Label.Content = Source.HOMO.EnergyString(4);
            HOMO_Label.Margin = new Thickness(0, HOMO_Line.Y1, 0, 0);
            LUMO_Label.Content = Source.LUMO.EnergyString(4);
            LUMO_Label.Margin = new Thickness(0, LUMO_Line.Y1 - 40, 0, 0);
            Delta_Label.Content = Source.GapString(4);
            Delta_Label.Margin = new Thickness(Delta_Label.Margin.Left, 
                (HOMO_Line.Y1 - LUMO_Line.Y1 - Delta_Label.ActualHeight)/2 + LUMO_Line.Y1, 0, 0);

            Arrow_Line.Y1 = LUMO_Line.Y1;
            Arrow_Line.Y2 = HOMO_Line.Y1;

            Arrow_Up.Points = new PointCollection()
            {
                new Point(70, LUMO_Line.Y1),
                new Point(65, LUMO_Line.Y1+10),
                new Point(75, LUMO_Line.Y1+10)
            };

            Arrow_Down.Points = new PointCollection()
            {
                new Point(70, HOMO_Line.Y1),
                new Point(65, HOMO_Line.Y1-10),
                new Point(75, HOMO_Line.Y1-10)
            };

            HOMO_Image.Margin = new Thickness(0, HOMO_Label.Margin.Top + HOMO_Label.ActualHeight, 0, 0);
            LUMO_Image.Margin = new Thickness(0, LUMO_Label.Margin.Top - LUMO_Image.ActualHeight, 0, 0);
        }
    }
}
