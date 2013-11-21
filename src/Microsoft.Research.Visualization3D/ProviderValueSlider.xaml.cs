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
using Microsoft.Research.Visualization3D.RayCasting;

namespace Microsoft.Research.Visualization3D
{
    /// <summary>
    /// Interaction logic for RayCastingSlicer.xaml
    /// </summary>
    public partial class ValueSlider : UserControl
    {
        public string HeaderName
        {
            get
            {
                return header.Header.ToString();
            }
            set
            {
                header.Header = value;
            }
        }
     
        private double min;

        public double Min
        {
            get { return min; }
            set 
            { 
                min = value;
                rcSlider.Minimum = min;
                minTB.Text = "Min = " + value.ToString("F2");
            }
        }
        private double max;

        public double Max
        {
            get { return max; }
            set 
            { 
                max = value;
                rcSlider.Maximum = max;
                maxTB.Text = "Max = " + value.ToString("F2");
            }
        }

        private DrawableComponent rayCastingProvider;

        public DrawableComponent Provider
        {
            get { return rayCastingProvider; }
            set 
            { 
                rayCastingProvider = value;
                Min = value.DataSource.Minimum;
                Max = value.DataSource.Maximum;
                rcSlider.Value = value.CurrentValue;
                rcTB.Text = "Value = " + value.CurrentValue.ToString("F2");
                rcChBox.IsEnabled = true;
                if (value is RayCastingProvider)
                    rcChBox.IsChecked = (rayCastingProvider as RayCastingProvider).EnableSlicing;
                else
                    rcChBox.IsChecked = true;
                rcSlider.IsEnabled = (bool)rcChBox.IsChecked;
            }
        }

        public event RoutedPropertyChangedEventHandler<double> ValueChanged; 
        
        public ValueSlider()
        {
            InitializeComponent();

            rcSlider.ValueChanged += RaiseValueChanged;
            rcSlider.Value = (max + min) / 2.0;
            rcSlider.IsEnabled = false;
            rcChBox.IsEnabled = false;
            
        }

        void RaiseValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValueChanged != null)
                ValueChanged(this, e);

            rcTB.Text = "Value = " + rcSlider.Value.ToString("F2");

            if (rayCastingProvider != null)
            {
                rayCastingProvider.CurrentValue = (float)rcSlider.Value;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (rayCastingProvider is RayCastingProvider)
            {
                (rayCastingProvider as RayCastingProvider).EnableSlicing = (bool)rcChBox.IsChecked;
            }
            rcSlider.IsEnabled = (bool)rcChBox.IsChecked;
        }

        

        
    }
}
