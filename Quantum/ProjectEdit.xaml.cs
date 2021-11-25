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
        private SQLiteDataBase DB;
        
        public ProjectEdit()
        {
            InitializeComponent();
        }

        public static void JustShow(SQLiteDataBase db)
        {
            ProjectEdit PE = new ProjectEdit() { DB = db };
            Node Input = Node.LoadFromDB(PE.DB, 1);
            PE.Panel.Children.Add(Input);
            Input.LoadChildren();
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

        private void RunInputBtn_Click(object sender, RoutedEventArgs e)
        {
            RunData RD = RunWindow.NewRun();
            if (RD.Text == null) return;
            Node NewNode = AddNode(NodeType.Run, RD.Text, "");
            NewNode.ParallelRun = RD.Parallel;
        }

        private void AddOptBtn_Click(object sender, RoutedEventArgs e)
        {
            Job NewJob = JobEdit.New(this, DB);
            if (NewJob == null) return;
            Node NewNode = AddNode(NodeType.Optimization, $"Узел {NodeN++}", "Расчётный узел");
            NewNode.NodeJob = NewJob;
        }

        private void AddEndBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNode(NodeType.End, $"Узел {NodeN++}", "Параметрический узел");
        }

        private void AddCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            AddNode(NodeType.End, $"Комментарий", "");
        }

        private Node AddNode(NodeType Type, string Title, string Comment)
        {
            Node NewNode = new Node()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Title = Title,
                Info = Comment,
                Type = Type
            };

            Panel.Children.Add(NewNode);
            NewNode.Save(DB);

            return NewNode;
        }

        private void Panel_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine($"Key {e.Key} is Down");

            switch (e.Key)
            {
                case Key.Delete:
                    if (MessageBox.Show("Вы уверены, что хотите удалить выделенные узлы?", "Удаление узлов", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        Panel.DeleteSelectedNotes();
                    break;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Panel.CreateProject();
        }
    }
}
