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
        /// <param name="name">Заголовок поля ввода названия</param>
        /// <param name="value">Заголовок поля ввода значения</param>
        /// <returns></returns>
        public static KeyValuePair<string, string> Add(string Title, string name = "Название", string value = "Значение")
        {
            AddParam AP = new AddParam()
            {
                Title = Title                
            };
            AP.ParamNameLabel.Content = name;
            AP.ParamValueLabel.Content = value;
            AP.ShowDialog();
            return AP.Output;
        }

        /// <summary>
        /// Вызывает окно добавления одного значения
        /// </summary>
        /// <param name="Title">Заголовок окна</param>
        /// <param name="value">Заголовок поля ввода значения</param>
        /// <returns></returns>
        public static string AddString(string Title, string value = "Значение")
        {
            AddParam AP = new AddParam()
            {
                Title = Title
            };

            AP.NameGrid.Visibility = Visibility.Collapsed;
            AP.ParamValueLabel.Content = value;
            AP.ShowDialog();
            return AP.Output.Value;
        }

        /// <summary>
        /// Вызывает окно изменения параметра и его значения
        /// </summary>
        /// <param name="Title">Заголовок окна</param>
        /// <param name="name">Заголовок поля ввода названия</param>
        /// <param name="value">Заголовок поля ввода значения</param>
        /// <returns></returns>
        public static KeyValuePair<string, string> Edit(string Title, KeyValuePair<string, string> Data, string name = "Название", string value = "Значение")
        {
            AddParam AP = new AddParam()
            {
                Title = Title
            };
            AP.ParamNameLabel.Content = name;
            AP.ParamValueLabel.Content = value;
            AP.NameTB.Text = Data.Key;
            AP.CodeTB.Text = Data.Value;
            AP.ShowDialog();
            return AP.Output;
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
