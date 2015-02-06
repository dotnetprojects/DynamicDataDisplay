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
using Microsoft.Research.DynamicDataDisplay;

namespace LayoutTutorial
{
    public partial class Page : UserControl
    {
        private Header additioanlHeader;
        private Header mainHeader;
        private Footer footer;
        private VerticalAxisTitle vertTitle;
        private HorizontalAxisTitle horiTitle;

        public Page()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainHeader = new Header();
            TextBlock anotherHeaderText = new TextBlock();
            anotherHeaderText.Text = "Layout Tutorial";
            anotherHeaderText.TextAlignment = TextAlignment.Center;
            anotherHeaderText.FontSize = 22;
            mainHeader.Content = anotherHeaderText;
            PlotterMain.Children.Add(mainHeader);

            additioanlHeader = new Header();
            TextBlock headerText = new TextBlock();
            headerText.Text = "Additional header";
            headerText.FontSize = 20;
            headerText.FontWeight = FontWeights.Bold;
            additioanlHeader.Content = headerText;
            PlotterMain.Children.Add(additioanlHeader);

            
            footer = new Footer();
            TextBlock footerText = new TextBlock();
            footerText.Text = "Small footer";
            footerText.FontSize = 8;
            footerText.FontStyle = FontStyles.Italic;
            footer.Content = footerText;

            
            TextBlock vertTitleTextBlock = new TextBlock();
            vertTitleTextBlock.Text = "This is Vertical title";
            //Only VerticalAxisTitle should be created this way, to create HorizonatalAxisTitle use its Content property
            //DO NOT use Content property in VerticalAxisTitle
            vertTitle = new VerticalAxisTitle(vertTitleTextBlock);
            PlotterMain.Children.Add(vertTitle);

            //Use Content property of the HorizontalAxisTitle to set its content
            horiTitle = new HorizontalAxisTitle();
            horiTitle.Content = new TextBlock() { Text="Horizontal axis title set from code",HorizontalAlignment= HorizontalAlignment.Center};
            PlotterMain.Children.Add(horiTitle);

            TextBoxUpperHeaderTitle.Text = anotherHeaderText.Text;
            TextBoxVerticalTitle.Text = vertTitleTextBlock.Text;
        }

        #region some handlers
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (PlotterMain.Children.Contains(additioanlHeader))
            {
                CheckboxAdditionalHeader.IsChecked = false;
                PlotterMain.Children.Remove(additioanlHeader);
            }
            else
            {
                CheckboxAdditionalHeader.IsChecked = true;
                PlotterMain.Children.Add(additioanlHeader);
            }
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            if (PlotterMain.Children.Contains(footer))
            {
                CheckboxAdditionalFooter.IsChecked = false;
                PlotterMain.Children.Remove(footer);
            }
            else
            {
                CheckboxAdditionalFooter.IsChecked = true;
                PlotterMain.Children.Add(footer);
            }
        }

        private void ButtonChangeVertAxis_CLick(object sender, RoutedEventArgs e)
        {
            if (vertTitle != null)
                PlotterMain.Children.Remove(vertTitle);

            

            TextBlock newTitleTextBlock = new TextBlock();
            newTitleTextBlock.Text = TextBoxVerticalTitle.Text;

            vertTitle = new VerticalAxisTitle(newTitleTextBlock);

            PlotterMain.Children.Add(vertTitle);

        }

        private void ButtonChangeUpperHeader_CLick(object sender, RoutedEventArgs e)
        {
            TextBlock newTextBlock = new TextBlock();
            newTextBlock.Text = TextBoxUpperHeaderTitle.Text;
            newTextBlock.TextAlignment = TextAlignment.Center;
            newTextBlock.FontSize = 22;
            mainHeader.Content = newTextBlock;
        }
        #endregion
    }
}
