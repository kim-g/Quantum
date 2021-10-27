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
    /// Логика взаимодействия для AddParam.xaml
    /// </summary>
    public partial class AddParam : Window
    {
        private KeyValuePair<string, string> Output = new KeyValuePair<string, string>(null, null);
        
        public AddParam()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Вызывает окно добавления параметра и его значения
        /// </summary>
        /// <param name="Title">Заголовок окна</param>
        /// <returns></returns>
        public static KeyValuePair<string, string> Add(string Title)
        {
            AddParam AP = new AddParam()
            {
                Title = Title
            };
            AP.ShowDialog();
            return AP.Output;
        }

        public static string AddString(string Title)
        {
            AddParam AP = new AddParam()
            {
                Title = Title
            };

            AP.NameGrid.Visibility = Visibility.Collapsed;
            AP.ShowDialog();
            return AP.Output.Value;
        }

        private void CancelB_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddB_Click(object sender, RoutedEventArgs e)
        {
            Output = new KeyValuePair<string, string>(NameTB.Text, CodeTB.Text);
            Close();
        }
    }
}
