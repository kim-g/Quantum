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
        public ProjectEdit()
        {
            InitializeComponent();
            Node2.ParentNode = Node1;
            Node3.ParentNode = Node1;

            Node1.RePaint();
        }

        private void Node1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Node1.RePaint();
        }
    }
}
