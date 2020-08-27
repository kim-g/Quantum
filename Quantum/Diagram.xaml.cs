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
    /// Логика взаимодействия для Diagram.xaml
    /// </summary>
    public partial class Diagram : UserControl
    {
        protected Brush diagrambackground;
        protected Brush legendbackground;
        protected List<EnergyElement> source;
        protected int SourceElementsOld = 0;
        
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
        public List<EnergyElement> Source
        {
            get => source;
            set
            {
                source = value;
            }
        }

        public Diagram()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обновляет диаграмму
        /// </summary>
        public void Update()
        {
            int Diff = Source.Count - SourceElementsOld;
            if (Diff > 0)
            {

            }
            if (Diff < 0)
            {

            }
            SourceElementsOld = Source.Count;

            int i = 0;
            foreach (EnergyElement EE in Source)
            {

            }
        }
    }

    public class Lines
    {
        public Line HOMO_Line = new Line();
        public Line LUMO_Line = new Line();
        public Line Arrow = new Line();
        public Image HOMO_Image = new Image();
        public Image LUMO_Image = new Image();
        public Label Name = new Label();
        public Label HOMO_Label = new Label();
        public Label LUMO_Label = new Label();
        public Label Delta_Label = new Label();
        public Polygon Arrow_Up = new Polygon();
        public Polygon Arrow_Down= new Polygon();
    }
}
