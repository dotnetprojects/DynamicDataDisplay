using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplaySilverLight.DataSources;
using System.Windows.Media.Imaging;
using System.IO;

namespace ImageHistogram
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }

        private void OnOpenImageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image|*.bmp;*.jpg;*.png;*.gif";
            dlg.ShowDialog();

            BitmapImage imageSource = new BitmapImage();

            Stream stream = dlg.File.OpenRead();

            BinaryReader binaryReader = new BinaryReader(stream);

            byte[] currentImageInBytes = binaryReader.ReadBytes((int)stream.Length);

            stream.Position = 0;

            imageSource.SetSource(stream);

            this.ImagePicture.Source = imageSource;

            ProcessImage(bmp);
        }

        EnumerableDataSource<int> red;
        EnumerableDataSource<int> green;
        EnumerableDataSource<int> blue;

        int[] reds = new int[256];
        int[] greens = new int[256];
        int[] blues = new int[256];
        byte[] pixels;
        private void ProcessImage(byte[] pixels)
        {
            

            for (int i = 0; i < pixels.Length; )
            {
                //BGRA
                blues[pixels[i++]]++;
                greens[pixels[i++]]++;
                reds[pixels[i++]]++;
                i++;
            }

            CreateHistograms();
        }

        private void CreateHistograms()
        {
            EnumerableDataSource<int> x = new EnumerableDataSource<int>(Enumerable.Range(0, 256).ToArray());
            x.SetXMapping(_x => _x);

            Func<int, double> mapping;
            if (check.IsChecked.GetValueOrDefault())
                mapping = logMapping;
            else
                mapping = linearMapping;

            red = new EnumerableDataSource<int>(reds);
            red.SetYMapping(mapping);
            green = new EnumerableDataSource<int>(greens);
            green.SetYMapping(mapping);
            blue = new EnumerableDataSource<int>(blues);
            blue.SetYMapping(mapping);

            CompositeDataSource rDS = new CompositeDataSource(x, red);
            CompositeDataSource gDS = new CompositeDataSource(x, green);
            CompositeDataSource bDS = new CompositeDataSource(x, blue);

            plotter.RemoveAllGraphs();
            plotter.AddLineGraph(rDS, Colors.Red, 1, "Red").FilteringEnabled = false;
            plotter.AddLineGraph(gDS, Colors.Green, 1, "Green").FilteringEnabled = false;
            plotter.AddLineGraph(bDS, Colors.Blue, 1, "Blue").FilteringEnabled = false;
        }

        private Func<int, double> logMapping = i => i > 0 ? Math.Log10(i) : 0;
        private Func<int, double> linearMapping = i => i;

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Func<int, double> mapping;
            if (check.IsChecked.GetValueOrDefault())
                mapping = logMapping;
            else
                mapping = linearMapping;

            red.SetYMapping(mapping);
            green.SetYMapping(mapping);
            blue.SetYMapping(mapping);
        }
    }
}
