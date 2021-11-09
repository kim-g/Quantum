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
using System.Windows.Shapes;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для ProjectEdit.xaml
    /// </summary>
    public partial class ProjectEdit : Window
    {
        private int NodeN = 1;
        
        public ProjectEdit()
        {
            InitializeComponent();
        }

        public static void JustShow()
        {
            ProjectEdit PE = new ProjectEdit();
            PE.ShowDialog();
        }

        private void Node1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((Node)sender).RePaint();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            foreach (Node N in Panel.Children.OfType<Node>())
                N.RePaint();
        }

        private void AddInputBtn_Click(object sender, RoutedEventArgs e)
        {
            Node NewNode = new Node()
            {
                Margin = new Thickness(0,0,0,0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top, 
                Title = $"INPUT",
                Info = "",
                Type = NodeType.Input
            };

            Panel.Children.Add(NewNode);
        }

        private void AddOptBtn_Click(object sender, RoutedEventArgs e)
        {
            Node NewNode = new Node()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Title = $"Узел {NodeN++}",
                Info = "Расчётный узел",
                Type = NodeType.Optimization
            };

            Panel.Children.Add(NewNode);
        }

        private void AddEndBtn_Click(object sender, RoutedEventArgs e)
        {
            Node NewNode = new Node()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Title = $"Узел {NodeN++}",
                Info = "Параметрический узел",
                Type = NodeType.End
            };

            Panel.Children.Add(NewNode);
        }

        private void AddCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            Node NewNode = new Node()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Title = $"Комментарий",
                Info = "",
                Type = NodeType.Comment
            };

            Panel.Children.Add(NewNode);
        }
    }
}
