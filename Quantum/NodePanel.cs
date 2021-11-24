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
        private Node connectionParent = null;
        private Node connectionChild = null;

        private Line ConnectionLine = new Line() { Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 1.5, Visibility = Visibility.Hidden };

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

        public NodePanel()
        {
            Background = new LinearGradientBrush(Color.FromRgb(0xEE, 0xEE, 0xEE), Color.FromRgb(0xC7, 0xC7, 0xC7), 90);
            Children.Add(new Rectangle() { Stroke = new SolidColorBrush(Colors.DarkGray), StrokeThickness = 1 });
            Children.Add(ConnectionLine);

            MouseMove += NodePanel_MouseMove;
            MouseLeftButtonUp += NodePanel_LeftMouseUp;
        }


        private void NodePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Handled) return;
            e.Handled = true;

            Point Position = e.GetPosition(this);

            foreach (Node N in Children.OfType<Node>().Where(x => x.Drag))
                Move(N.MoveDelta(Position));

            if (ConnectionParent!= null || ConnectionChild != null)
            {
                ConnectionLine.X2 = Position.X;
                ConnectionLine.Y2 = Position.Y;
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


    }
}
