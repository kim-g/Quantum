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
        private Brush linebrush = new SolidColorBrush(Colors.Green);
        private Point StartDrag;
        private string NameText;
        private string InfoText;
        private NodeType type = NodeType.Optimization;


        private Brush InputBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(0xE6,0xE6,0x00), 0.0),
            new GradientStop(Color.FromRgb(0x83,0x83,0x00), 0.063),
            new GradientStop(Color.FromRgb(0x4D,0x4D,0x00), 0.937),
            new GradientStop(Color.FromRgb(0x2B,0x2B,0x00), 1.0)
        }), 90);
        private Brush OptimizationBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() 
        { 
            new GradientStop(Color.FromRgb(0x34,0xE6,0x00), 0.0),
            new GradientStop(Color.FromRgb(0x1D,0x83,0x00), 0.063),
            new GradientStop(Color.FromRgb(0x11,0x4D,0x00), 0.937),
            new GradientStop(Color.FromRgb(0x0A,0x2B,0x00), 1.0)
        }), 90);
        private Brush StopBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(0xE6,0x34,0x00), 0.0),
            new GradientStop(Color.FromRgb(0x83,0x1D,0x00), 0.063),
            new GradientStop(Color.FromRgb(0x4D,0x11,0x00), 0.937),
            new GradientStop(Color.FromRgb(0x2B,0x0A,0x00), 1.0)
        }), 90);
        private Brush CommentBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(0x00,0x00,0xE6), 0.0),
            new GradientStop(Color.FromRgb(0x00,0x00,0x83), 0.063),
            new GradientStop(Color.FromRgb(0x00,0x00,0x4D), 0.937),
            new GradientStop(Color.FromRgb(0x00,0x00,0x2B), 1.0)
        }), 90);


        private List<Line> ChildrenLines = new List<Line>();

        /// <summary>
        /// Родительский узел. Может быть только один.
        /// </summary>
        public Node ParentNode
        {
            get => parent;
            set
            {
                if (parent != null)
                {
                    parent.ChildrenNodes.Remove(this);
                    ((NodePanel)Parent).Children.Remove(parent.ChildrenLines[parent.ChildrenLines.Count() - 1]);
                    parent.ChildrenLines.RemoveAt(parent.ChildrenLines.Count() - 1);
                }
                parent = value;
                ParentRect.Fill = parent == null ? null : parentbrush;
                if (parent == null) ParentLine = null;
                else
                {
                    ParentLine = new Line() { Stroke = linebrush, StrokeThickness = 1.5 };
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
        /// Положение стыковки для родительского узла
        /// </summary>
        public Point ParentPosition {
            get
            {
                return new Point(Margin.Left + ParentRect.ActualWidth / 2, Margin.Top + ActualHeight / 2);
            }
        }

        /// <summary>
        /// Положение стыковки для узлов-потомков
        /// </summary>
        public Point ChildrenPosition
        {
            get
            {
                return new Point(Margin.Left + ActualWidth - ParentRect.ActualWidth / 2, Margin.Top + ActualHeight / 2);
            }
        }

        /// <summary>
        /// Должен ли у узла быть родитель
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
        /// Должны ли у узла быть потомки
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
        /// Заголовок узла.
        /// </summary>
        public string Title
        {
            get => NameText;
            set
            {
                NameText = value;
                NameLabel.Content = NameText;
            }
        }

        /// <summary>
        /// Подробная информация об узле
        /// </summary>
        public string Info
        {
            get => InfoText;
            set
            {
                InfoText = value;
                InfoBlock.Text = InfoText;
                InfoBlock.Visibility = InfoText == "" ? Visibility.Collapsed : Visibility.Visible;
                this.Height = RealHeight();
            }
        }

        public bool Drag { get; private set; }

        public NodeType Type
        {
            get => type;
            set
            {
                type = value;
                switch(type)
                {
                    case NodeType.Input:
                        HasParent = false;
                        HasChildren = true;
                        FillRect.RadiusX = 10;
                        FillRect.RadiusY = 10;
                        FillRect.Fill = InputBrush;
                        Width = 80;
                        break;
                    case NodeType.Optimization:
                        HasParent = true;
                        HasChildren = true;
                        FillRect.RadiusX = 3;
                        FillRect.RadiusY = 3;
                        FillRect.Fill = OptimizationBrush;
                        Width = 150;
                        break;
                    case NodeType.End:
                        HasParent = true;
                        HasChildren = false;
                        FillRect.RadiusX = 3;
                        FillRect.RadiusY = 3;
                        FillRect.Fill = StopBrush;
                        Width = 150;
                        break;
                    case NodeType.Comment:
                        HasParent = true;
                        HasChildren = false;
                        FillRect.RadiusX = 3;
                        FillRect.RadiusY = 3;
                        FillRect.Fill = CommentBrush;
                        Width = 150;
                        break;
                }
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
                ParentLine.X2 = MyPoint.X;
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

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartDrag = e.GetPosition((IInputElement)Parent);
            Drag = true;
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StopDrag();
        }

        public void StopDrag()
        {
            Drag = false;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (Drag)
            {
                Move(e.GetPosition((IInputElement)Parent));
            }
        }

        public void Move(Point CurrentPosition)
        {
            double X = Margin.Left + CurrentPosition.X - StartDrag.X;
            if (X < 0) X = 0;
            if (X > ((FrameworkElement)Parent).ActualWidth - ActualWidth) X = ((FrameworkElement)Parent).ActualWidth - ActualWidth;
            double Y = Margin.Top + CurrentPosition.Y - StartDrag.Y;
            if (Y < 0) Y = 0;
            if (Y > ((FrameworkElement)Parent).ActualHeight - ActualHeight) Y = ((FrameworkElement)Parent).ActualHeight - ActualHeight;
            Margin = new Thickness(X, Y, 0, 0);
            StartDrag = CurrentPosition;
            RePaint();
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Height = RealHeight();
            Console.WriteLine($"ActualHeight = {ActualHeight}");
        }

        private double RealHeight()
        {
            double Res = Data.Margin.Top + Data.Margin.Bottom + 
                NameLabel.ActualHeight + NameLabel.Margin.Top + NameLabel.Margin.Bottom + 
                InfoBlock.ActualHeight + InfoLabel.Padding.Top + InfoLabel.Padding.Bottom;
            return Res;
        }

        private void ChildrenRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((NodePanel)Parent).ConnectionParent = this;
        }

        private void ChildrenRect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (((NodePanel)Parent).ConnectionParent == this) ((NodePanel)Parent).ConnectionParent = null;

            if (((NodePanel)Parent).ConnectionChild != null)
            {
                ((NodePanel)Parent).ConnectionChild.ParentNode = this;
                ((NodePanel)Parent).ConnectionChild = null;
            }
        }

        private void ParentRect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (((NodePanel)Parent).ConnectionChild == this) ((NodePanel)Parent).ConnectionChild = null;

            if (((NodePanel)Parent).ConnectionParent != null)
            {
                ParentNode = ((NodePanel)Parent).ConnectionParent;
                ((NodePanel)Parent).ConnectionParent = null;
            }
        }

        private void ParentRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((NodePanel)Parent).ConnectionChild = this;
        }
    }

    public enum NodeType { Input, Optimization, End, Comment }
}
