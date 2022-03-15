using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

        /// <summary>
        /// Редактирование проекта
        /// </summary>
        /// <param name="db"></param>
        /// <param name="StartNode"></param>
        public static void Edit(SQLiteDataBase db, long ProjectID)
        {
            Project P = Project.Load(db, ProjectID);
            
            ProjectEdit PE = new ProjectEdit() { DB = db };
            PE.NameTB.Text = P.Title;
            PE.DescriptionTB.Text = P.Description;
            Node Input = Node.LoadFromDB(PE.DB, P.Input);
            PE.Panel.Children.Add(Input);
            Input.LoadChildren();
            PE.ShowDialog();
            P.Title = PE.NameTB.Text;
            P.Description = PE.DescriptionTB.Text;
        }

        private void Node1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((Node)sender).RePaint();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            foreach (Node N in Panel.Nodes)
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

        private void AddCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, string> Comment = AddParam.Add("Добавить комментарий", "Заголовок", "Содержание");
            if (Comment.Key == null) return;
            AddNode(NodeType.Comment, Comment.Key, Comment.Value);
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine($"Key {e.Key} is Down");

            switch (e.Key)
            {
                case Key.Delete:
                    if (MessageBox.Show("Вы уверены, что хотите удалить выделенные узлы?", "Удаление узлов", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        Panel.DeleteSelectedNotes();
                    break;
                case Key.D:
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    {
                        List<Node> ToDublicate = Panel.Selected;
                        foreach (Node N in ToDublicate)
                        {
                            Node NewNode = N.Dublicate();
                            Panel.Children.Add(NewNode);
                            N.Selected = false;
                            NewNode.Selected = true;
                        }
                    }
                    break;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Panel.CreateProject();
        }
    }
}
