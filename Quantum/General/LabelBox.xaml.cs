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
    /// Логика взаимодействия для LabelBox.xaml
    /// </summary>
    public partial class LabelBox : UserControl
    {
        /// <summary>
        /// Заголовок элемента
        /// </summary>
        public string Title
        {
            get => NameLabel.Content.ToString();
            set => NameLabel.Content = value;
        }

        public string Text
        {
            get => TextEdit.Text;
            set => TextEdit.Text = value;
        }

        public LabelBox()
        {
            InitializeComponent();
        }
    }
}
