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
    /// Логика взаимодействия для MoleculeEdit.xaml
    /// </summary>
    public partial class MoleculeEdit : Window
    {
        private bool OK = false;
        
        public MoleculeEdit()
        {
            InitializeComponent();
        }

        private void LUMO_Img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LUMO_Img.Source = SwapClipboardImage();
        }

        private void HOMO_Img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HOMO_Img.Source = SwapClipboardImage();
        }

        public static EnergyElement Add()
        {
            MoleculeEdit ME = new MoleculeEdit();
            ME.ShowDialog();

            if (ME.OK)
            {
                char Separatop = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

                EnergyElement EE = new EnergyElement();
                EE.Name = ME.MolNameTB.Text;
                EE.HOMO.Picture = ME.HOMO_Img.Source;
                EE.LUMO.Picture = ME.LUMO_Img.Source;
                EE.HOMO.Energy = Convert.ToDouble(ME.HOMO_Value.Text.Replace('.', Separatop).Replace(',', Separatop));
                EE.LUMO.Energy = Convert.ToDouble(ME.LUMO_Value.Text.Replace('.', Separatop).Replace(',', Separatop));

                return EE;
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OK = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Получить изображение из буфера обмена
        /// </summary>
        /// <returns></returns>
        public BitmapSource SwapClipboardImage()
        {
            BitmapSource returnImage = null;

            if (Clipboard.ContainsImage())
            {
                System.Windows.Forms.IDataObject clipboardData = System.Windows.Forms.Clipboard.GetDataObject();
                if (clipboardData != null)
                {
                    if (clipboardData.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
                    {
                        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)clipboardData.GetData(System.Windows.Forms.DataFormats.Bitmap);
                        returnImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        Console.WriteLine("Clipboard copied to UIElement");
                    }
                }
            }
            return returnImage;
        }
    }
}
