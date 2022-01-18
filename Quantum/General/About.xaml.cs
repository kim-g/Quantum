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
    /// Логика взаимодействия для About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            Version.Text = MainMenu.ProgramVersion;
            Year.Text = DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// Показать окно с информацией о программе и её авторах в диалоговом режиме
        /// </summary>
        /// <param name="owner">Окно-родитель</param>
        public static void ShowModal(Window owner)
        {
            About AboutForm = new About();
            AboutForm.Owner = owner;
            AboutForm.ShowDialog();
            GC.Collect();
        }

        private void Author_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:kim-g@ios.uran.ru");
        }

        private void Link_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Run)sender).TextDecorations = TextDecorations.Underline;
        }

        private void Link_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Run)sender).TextDecorations = null;
        }

        private void GitHub_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/kim-g/Quantum");
        }

        private void License_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/kim-g/Quantum/blob/master/LICENSE");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
