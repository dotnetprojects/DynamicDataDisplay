//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;

//namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine.Functional
//{
//    public sealed class FunctionalDataSource : FunctionalDataSourceBase
//    {
//        public Func<double, double> Function
//        {
//            get { return (Func<double, double>)GetValue(FunctionProperty); }
//            set { SetValue(FunctionProperty, value); }
//        }

//        public static readonly DependencyProperty FunctionProperty = DependencyProperty.Register(
//          "Function",
//          typeof(Func<double, double>),
//          typeof(FunctionalDataSource),
//          new FrameworkPropertyMetadata((Func<double, double>)(x => x), OnFunctionChanged),
//          OnValidateFunction
//          );

//        private static bool OnValidateFunction(object value)
//        {
//            return value != null;
//        }

//        private static void OnFunctionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            FunctionalDataSource ds = (FunctionalDataSource)d;
//            ds.FunctionCore = (Func<double, double>)e.NewValue;
//        }

//        protected override IEnumerable<double> GetXValues()
//        {
//            var visible = DataSource2dContext.GetVisibleRect(this);
//            var screen = DataSource2dContext.GetScreenRect(this);

//            int count = (int)Math.Ceiling(screen.Width) + 1;

//            if (count == 0)
//                yield break;
//            else
//            {
//                double delta = visible.Width / count;
//                double x = (int)(visible.XMin / delta - 1) * delta;
//                for (int i = 0; i < count; i++)
//                {
//                    yield return x + i * delta;
//                }
//            }
//        }
//    }
//}
