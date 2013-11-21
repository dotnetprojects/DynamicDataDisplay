using System;
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
using System.Xml.Linq;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom;

namespace DeepZoomSample
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //XmlReaderSettings settings = new XmlReaderSettings();
            //XmlSchema schema = null;
            //using (FileStream fs = new FileStream(@"..\..\DeepZoomSchema.xsd", FileMode.Open))
            //{
            //    schema = XmlSchema.Read(fs, new ValidationEventHandler(SchemaValidationCallback));
            //}
            //settings.Schemas.Add(schema);

            string imgPath = Environment.CommandLine.Replace(@"C:\Development\SS\DynamicDataDisplay\Main\src\DevSamples\DeepZoomSample\bin\Debug\DeepZoomSample.vshost.exe", "")
                .Trim('"').Trim(' ');

            viewer.ImagePath = imgPath;
        }

        private void SchemaValidationCallback(object sender, ValidationEventArgs e) { }
    }
}
