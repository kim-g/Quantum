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
    /// Логика взаимодействия для GetImageForm.xaml
    /// </summary>
    public partial class GetImageForm : Window
    {
        public GetImageForm()
        {
            InitializeComponent();
        }

        public static ImageSource LoadImage()
        {
            GetImageForm GIF = new GetImageForm();
            GIF.ShowDialog();
            return GIF.GotImage.Source;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GotImage.Source = SwapClipboardImage();
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
