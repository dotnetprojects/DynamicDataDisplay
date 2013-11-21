//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Functional
//{
//    public sealed class ConstantDataSource : FunctionalDataSourceBase
//    {
//        public ConstantDataSource() { }
//        public ConstantDataSource(double constantValue)
//        {
//            this.Value = constantValue;
//        }

//        private void UpdateFunction()
//        {
//            double value = Value;
//            FunctionCore = x => value;
//        }

//        protected override IEnumerable<double> GetXValues()
//        {
//            DataRect visible = DataSource2dContext.GetVisibleRect(this);

//            yield return visible.XMin;
//            yield return visible.XMax;
//        }

//        public double Value
//        {
//            get { return (double)GetValue(ValueProperty); }
//            set { SetValue(ValueProperty, value); }
//        }

//        public static readonly DependencyProperty ValueProperty =
//            DependencyProperty.Register(
//              "Value",
//              typeof(double),
//              typeof(ConstantDataSource),
//              new PropertyMetadata(0.0, OnValueChanged));

//        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            ConstantDataSource ds = (ConstantDataSource)d;
//            ds.UpdateFunction();
//        }
//    }
//}
