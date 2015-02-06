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
using System.Collections.ObjectModel;
using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay
{
    
    [ContentProperty("Children")]
    public partial class Plotter : UserControl 
    {
        private bool alreadyLoaded=false;
        private ObservableCollection<IPlotterElement> children;

        public ObservableCollection<IPlotterElement> Children {
            get { return children; }
        }

        public StackPanel HoveringStackPanel {
            get {
                return hoveringStackPanel;
            }
        }
        
        public Plotter()
        {
            children = new ObservableCollection<IPlotterElement>();
            InitializeComponent();

           

            Loaded += new RoutedEventHandler(Plotter_Loaded);
        }

        void Plotter_Loaded(object sender, RoutedEventArgs e)
        {
            if (!alreadyLoaded)
            {
                foreach (IPlotterElement elem in children)
                {
                    elem.OnPlotterAttached(this);
                }
                children.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(children_CollectionChanged);
                CentralGrid.SizeChanged += new SizeChangedEventHandler(CentralGrid_SizeChanged);
                alreadyLoaded = true;

                
            }
            IsTabStop=true;
            Focus();
        }

        void CentralGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainCanvas.Width = CentralGrid.ActualWidth;
            MainCanvas.Height = CentralGrid.ActualHeight;
        }

        void children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null) {
                foreach (IPlotterElement elem in e.NewItems) elem.OnPlotterAttached(this);
            }
            if (e.OldItems != null)
            {
                foreach (IPlotterElement elem in e.OldItems) elem.OnPlotterDetaching(this);
            }
            
        }
    }
}
