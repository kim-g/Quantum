using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantum
{
    public class NodePanel : Grid
    {
        #region Внутренние переменные
        private Node connectionParent = null;
        private Node connectionChild = null;
        private Node mouseOn = null;

        private Line ConnectionLine = new Line() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1.5, Visibility = Visibility.Hidden };
        #endregion

        #region Свойства
        /// <summary>
        /// Узел-родитель при стыковке узлов
        /// </summary>
        public Node ConnectionParent
        {
            get => connectionParent;
            set
            {
                connectionParent = value;

                if (connectionParent == null)
                {
                    ConnectionLine.Visibility = Visibility.Hidden;
                    return;
                }

                ConnectionLine.X1 = connectionParent.ChildrenPosition.X;
                ConnectionLine.Y1 = connectionParent.ChildrenPosition.Y;
                ConnectionLine.X2 = connectionParent.ChildrenPosition.X;
                ConnectionLine.Y2 = connectionParent.ChildrenPosition.Y;
                ConnectionLine.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Узел-потомок при стыковке узлов
        /// </summary>
        public Node ConnectionChild
        {
            get => connectionChild;
            set
            {
                connectionChild = value;

                if (connectionChild == null)
                {
                    ConnectionLine.Visibility = Visibility.Hidden;
                    return;
                }

                ConnectionLine.X1 = connectionChild.ParentPosition.X;
                ConnectionLine.Y1 = connectionChild.ParentPosition.Y;
                ConnectionLine.X2 = connectionChild.ParentPosition.X;
                ConnectionLine.Y2 = connectionChild.ParentPosition.Y;
                ConnectionLine.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Нода, над которой находится мышь
        /// </summary>
        public Node MouseOn
        { 
            get => mouseOn; 
            set
            {
                mouseOn = value;
            }
        }

        /// <summary>
        /// Список сигналов. 
        /// </summary>
        public List<Sygnal> Sygnals { get; set; } = new List<Sygnal>();
        #endregion

        #region Конструктор
        public NodePanel()
        {
            Background = new LinearGradientBrush(Color.FromRgb(0xEE, 0xEE, 0xEE), Color.FromRgb(0xC7, 0xC7, 0xC7), 90);
            Children.Add(new Rectangle() { Stroke = new SolidColorBrush(Colors.DarkGray), StrokeThickness = 1 });
            Children.Add(ConnectionLine);

            MouseMove += NodePanel_MouseMove;
            MouseLeftButtonUp += NodePanel_LeftMouseUp;
        }
        #endregion

        #region События
        private void NodePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Handled) return;
            e.Handled = true;

            Point Position = e.GetPosition(this);

            foreach (Node N in Children.OfType<Node>().Where(x => x.Drag))
                Move(N.MoveDelta(Position));

            if (ConnectionParent!= null || ConnectionChild != null)
            {

                if (ConnectionParent == null)
                    if (MouseOn == null)
                    {
                        ConnectionLine.X2 = Position.X;
                        ConnectionLine.Y2 = Position.Y;
                    }
                    else
                    {
                        ConnectionLine.X2 = MouseOn.ChildrenPosition.X;
                        ConnectionLine.Y2 = MouseOn.ChildrenPosition.Y;
                    }
                if (ConnectionChild == null)
                    if (MouseOn == null)
                    {
                        ConnectionLine.X2 = Position.X;
                        ConnectionLine.Y2 = Position.Y;
                    }
                    else
                    {
                        ConnectionLine.X2 = MouseOn.ParentPosition.X;
                        ConnectionLine.Y2 = MouseOn.ParentPosition.Y;
                    }

            }
        }

        private void NodePanel_LeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled) return;
            
            foreach (Node N in Children.OfType<Node>().Where(x => x.Drag))
                N.StopDrag();

            foreach (Node N in Children.OfType<Node>().Where(x => x.Selected))
                    N.Selected = false;

            ConnectionParent = null;
            ConnectionChild = null;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Убирает выделените узлов
        /// </summary>
        public void DeselectAll()
        {
            foreach (Node N in Children.OfType<Node>().Where(x => x.Selected))
                N.Selected = false;
        }

        /// <summary>
        /// Удаляет узлы, вытирает из их БД, удаляет все связи.
        /// </summary>
        /// <param name="NodeList">Список узлов для удаления</param>
        public void DeleteNodes(List<Node> NodeList)
        {
            foreach (Node N in NodeList)
            {
                N.Delete();
                Children.Remove(N);
            }
        }

        /// <summary>
        /// Удаляет выделенные узлы
        /// </summary>
        public void DeleteSelectedNotes()
        {
            DeleteNodes(Children.OfType<Node>().Where(x => x.Selected && x.Type != NodeType.Input).ToList());
        }

        /// <summary>
        /// Сдвигает все выделенные узлы на указанное расстояние
        /// </summary>
        /// <param name="Delta">Сдвиг узлов</param>
        public void Move(Point Delta)
        {
            foreach (Node N in Children.OfType<Node>().Where(x => x.Selected))
                N.Move(Delta);
        }

        /// <summary>
        /// Сохраняет положение всех выделенных узлов
        /// </summary>
        public void SaveSelected()
        {
            foreach (Node N in Children.OfType<Node>().Where(x => x.Selected))
                N.StopDrag();
        }

        /// <summary>
        /// Создание файоа проекта
        /// </summary>
        public List<Sygnal> CreateProject()
        {
            Sygnal Start = Children.OfType<Node>().First(x => x.Type == NodeType.Input).StartSygnal();
            List<Sygnal> Sygnals = new List<Sygnal>();
            Sygnals.Add(Start);
            Sygnals.AddRange(Start.GetChildren());
            return Sygnals;
        }
        #endregion

    }
}
