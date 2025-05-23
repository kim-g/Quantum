﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextViewerFind;
using System.IO;
using System.Windows.Xps.Packaging;
using System.IO.Packaging;
using System.Windows.Xps;
using System.Globalization;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Interop;

namespace Quantum
{
    /// <summary>
    /// Преобразовывает элементы WPF в WMF
    /// </summary>
    public static class WPF_to_WMF_Converter
    {
        public static void CopyUIElementToClipboard(FrameworkElement element, float Scale)
        {
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width*3), (int)Math.Round(height*3), 96*3, 96*3, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }

        public static string CopyUIElementToXAML(FrameworkElement element)
        {
            return XamlWriter.Save(element);
        }

        public static void SaveUIElementToXAML(FrameworkElement element)
        {
            using (StreamWriter fs = new StreamWriter("test.xaml", false))
            {
                fs.Write(CopyUIElementToXAML(element));
            }
        }


        public static void CopyVisualToWmfClipboard(Visual visual, Window clipboardOwnerWindow)
        {
            MemoryStream xpsStream = new MemoryStream();

            Package package = Package.Open(xpsStream, FileMode.Create);

            XpsDocument doc = new XpsDocument(package);

            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(doc);

            xpsWriter.Write(visual);

            doc.Close();

            XpsDocument docPrime = new XpsDocument(package);

            IXpsFixedDocumentSequenceReader fixedDocSeqReader = docPrime.FixedDocumentSequenceReader;

            Dictionary<string, string> fontList = new Dictionary<string, string>();

            foreach (IXpsFixedDocumentReader docReader in fixedDocSeqReader.FixedDocuments)
            {
                int pageNum = 0;
                foreach (IXpsFixedPageReader fixedPageReader in docReader.FixedPages)
                {
                    while (fixedPageReader.XmlReader.Read())
                    {
                        string page = fixedPageReader.XmlReader.ReadOuterXml();
                        string path = string.Empty;

                        foreach (XpsFont font in fixedPageReader.Fonts)
                        {
                            string name = font.Uri.GetFileName();
                            path = string.Format(CultureInfo.InvariantCulture, @"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), name);

                            if (!fontList.ContainsKey(font.Uri.OriginalString))
                            {
                                fontList.Add(font.Uri.OriginalString, path);
                                font.SaveToDisk(path);
                            }
                        }

                        //foreach (XpsImage image in fixedPageReader.Images)
                        //{
                        //    //here to get images
                        //}

                        foreach (KeyValuePair<string, string> val in fontList)
                        {
                            page = page.Replace(val.Key, val.Value);
                            //RegEx not working right, the above should be pretty safe
                            //page = page.ReplaceAttribute("FontUri", val.Key,val.Value); 
                        }

                        FixedPage fp = XamlReader.Load(new MemoryStream(Encoding.Default.GetBytes(page))) as FixedPage;

                        XmlWriterSettings settings = new XmlWriterSettings();

                        settings.Indent = true;
                        settings.NewLineOnAttributes = true;

                        MemoryStream xpsPageStream = new MemoryStream();
                        XmlWriter writer = XmlWriter.Create(xpsPageStream, settings);

                        XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
                        manager.XamlWriterMode = XamlWriterMode.Expression;
                        manager.XamlWriterMode = XamlWriterMode.Value;

                        XamlWriter.Save(fp, manager);

                        xpsPageStream.Position = 0;

                        pageNum++;
                    }
                }
            }

            package.Close();

            try
            {
                // delete temp font files
                foreach (KeyValuePair<string, string> val in fontList)
                    File.Delete(val.Value);
            }
            catch
            {

            }
        }

        public static object LoadXamlFromStream(Stream stream)
        {
            using (Stream s = stream)
                return XamlReader.Load(s);
        }


        public static T GetDependencyObjectFromVisualTree<T>(DependencyObject startObject)
            // don't restrict to DependencyObject items, to allow retrieval of interfaces
            //where T : DependencyObject
            where T : class
        {
            //Walk the visual tree to get the parent(ItemsControl) 
            //of this control
            DependencyObject parent = startObject;
            while (parent != null)
            {
                T pt = parent as T;
                if (pt != null)
                    return pt;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        

    }
}
