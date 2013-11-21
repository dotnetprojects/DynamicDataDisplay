//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Research.DynamicDataDisplay.Charts;
//using Microsoft.Research.DynamicDataDisplay.Common;
//using DynamicDataDisplay.Markers.DataSources;
//using System.Collections.Specialized;
//using System.Windows;

//namespace DynamicDataDisplay.Markers
//{
//    internal sealed class PieChartLegendItemManager : IDisposable
//    {
//        private readonly PieChart chart;
//        private readonly NewLegend legend;

//        public PieChartLegendItemManager(PieChart chart, NewLegend legend)
//        {
//            if (chart == null)
//                throw new ArgumentNullException("chart");
//            if (legend == null)
//                throw new ArgumentNullException("legend");

//            this.chart = chart;
//            this.legend = legend;

//            chart.DataSourceChanged += chart_DataSourceChanged;
//            if (chart.DataSource != null)
//            {
//                chart.DataSource.CollectionChanged += OnDataSource_CollectionChanged;
//            }

//            UpdateLegendItems();
//        }

//        private void UpdateLegendItems()
//        {
//            legend.RemoveChart(chart);

//            if (chart.DataSource == null)
//                return;

//            foreach (PieChartItem item in chart.Items)
//            {
//                if (item.Visibility != Visibility.Visible)
//                    continue;

//                PieChartLegendItem legendItem = new PieChartLegendItem(item, chart);
//                //legend.AddLegendItem(chart, legendItem);
//            }
//        }

//        private void chart_DataSourceChanged(object sender, ValueChangedEventArgs<PointDataSourceBase> e)
//        {
//            if (e.PreviousValue != null)
//            {
//                e.PreviousValue.CollectionChanged -= OnDataSource_CollectionChanged;
//            }

//            if (e.CurrentValue != null)
//            {
//                e.CurrentValue.CollectionChanged += OnDataSource_CollectionChanged;
//            }

//            UpdateLegendItems();
//        }

//        private void OnDataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            UpdateLegendItems();
//        }

//        #region IDisposable Members

//        public void Dispose()
//        {

//        }

//        #endregion
//    }
//}
