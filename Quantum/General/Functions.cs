using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Quantum
{
    public class Functions
    {
       public static List<TextBox> Find_All_TextBoxes(Grid RootGrid)
        {
            List<TextBox> List = new List<TextBox>();

            // Ищем всё в корне
            foreach (TextBox TB in RootGrid.Children.OfType<TextBox>())
            {
                List.Add(TB);
            }

            // Ищем всё в дочерних гридах
            foreach (Grid ChildrenGrid in RootGrid.Children.OfType<Grid>())
            {
                List.AddRange(Find_All_TextBoxes(ChildrenGrid));
            }

            return List;
        }

        public static string OpenFile(string FileType = "Все файлы (*.*)|*.*")
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = FileType;
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;

            return myDialog.ShowDialog()==true ? myDialog.FileName : null;
        }
    }
}
