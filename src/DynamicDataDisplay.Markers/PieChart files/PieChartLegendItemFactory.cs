//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Research.DynamicDataDisplay.Charts;

//namespace DynamicDataDisplay.Markers
//{
//    internal sealed class PieChartLegendItemFactory
//    {
//        public IDisposable TryBuildItem(object chart, NewLegend legend)
//        {
//            PieChart pieChart = chart as PieChart;
//            if (pieChart == null)
//            {
//                ViewportUIContainer container = chart as ViewportUIContainer;
//                if (container != null)
//                {
//                    pieChart = container.Content as PieChart;
//                    if (pieChart == null)
//                        return null;
//                }
//                else
//                    return null;
//            }

//            PieChartLegendItemManager manager = new PieChartLegendItemManager(pieChart, legend);
//            return manager;
//        }
//    }
//}
