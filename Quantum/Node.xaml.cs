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
    /// Логика взаимодействия для Node.xaml
    /// </summary>
    public partial class Node : UserControl
    {
        private Node parent;
        private ObservableCollection<Node> children = new ObservableCollection<Node>();
        private bool hasparent = true;
        private bool haschildren = true;
        private Brush parentbrush = new SolidColorBrush(Colors.Red);

        private List<Line> ChildrenLines = new List<Line>();

        /// <summary>
        /// Родительский узел. Может быть только один.
        /// </summary>
        public Node ParentNode
        {
            get => parent;
            set
            {
                if (value == null) parent?.ChildrenNodes.Remove(this);
                parent = value;
                ParentRect.Fill = parent == null ? null : parentbrush;
                if (parent == null) ParentLine = null;
                else
                {
                    ParentLine = new Line() { Stroke = parentbrush, StrokeThickness = 2 };
                    ((Grid)this.Parent).Children.Add(ParentLine);
                    parent.ChildrenLines.Add(ParentLine);
                }

                parent?.ChildrenNodes.Add(this);
                ParentChanged?.Invoke(this, new EventArgs());

                RePaint();
            }
        }

        /// <summary>
        /// Линия, соединяющая родителя и потомка
        /// </summary>
        public Line ParentLine;

        /// <summary>
        /// Список узлов-потомков
        /// </summary>
        public ObservableCollection<Node> ChildrenNodes
        {
            get => children;
            set
            {
                children = value;
                children.CollectionChanged -= ChildrenListChanged;
                children.CollectionChanged += ChildrenListChanged;
                ChildrenChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Положение стыковки для родительской ноды
        /// </summary>
        public Point ParentPosition {
            get
            {
                return new Point(Margin.Left + ParentRect.ActualWidth / 2, Margin.Top + ActualHeight / 2);
            }
        }

        /// <summary>
        /// Положение стыковки для ноды потомков
        /// </summary>
        public Point ChildrenPosition
        {
            get
            {
                return new Point(Margin.Left + ActualWidth - ParentRect.ActualWidth / 2, Margin.Top + ActualHeight / 2);
            }
        }

        /// <summary>
        /// Должен ли у ноды быть родитель
        /// </summary>
        public bool HasParent
        {
            get => hasparent;
            protected set
            {
                hasparent = value;
                ParentRect.Visibility = hasparent
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Должны ли у ноды быть потомки
        /// </summary>
        public bool HasChildren
        {
            get => haschildren;
            protected set
            {
                haschildren = value;
                ChildrenRect.Visibility = haschildren
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Событие, возникающее при изменении родителя
        /// </summary>
        public event EventHandler ParentChanged;

        /// <summary>
        /// Событие, возникающее при любом изменении списка потомков
        /// </summary>
        public event EventHandler ChildrenChanged;

        public Node()
        {
            InitializeComponent();
        }

        private void ChildrenListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ChildrenChanged?.Invoke(this, new EventArgs());
        }

        public void RePaint()
        {
            if (ParentNode != null)
            {
                Point ParentPoint = ParentNode.ChildrenPosition;
                Point MyPoint = ParentPosition;
                ParentLine.X1 = ParentPoint.X;
                ParentLine.Y1 = ParentPoint.Y;
                ParentLine.X2 = MyPoint.Y;
                ParentLine.Y2 = MyPoint.Y;
            }

            Point MyChildrenPoint = ChildrenPosition;
            for (int i = 0; i < ChildrenNodes.Count; i++)
            {
                Point ChildrenPoint = ChildrenNodes[i].ParentPosition;
                ChildrenLines[i].X1 = MyChildrenPoint.X;
                ChildrenLines[i].Y1 = MyChildrenPoint.Y;
                ChildrenLines[i].X2 = ChildrenPoint.X;
                ChildrenLines[i].Y2 = ChildrenPoint.Y;
            }
        }
    }
}
