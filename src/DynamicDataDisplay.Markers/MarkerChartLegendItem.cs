//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Research.DynamicDataDisplay.Charts;
//using System.Windows;
//using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
//using System.Windows.Data;
//using Microsoft.Research.DynamicDataDisplay.Common;
//using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
//using Microsoft.Research.DynamicDataDisplay;

//namespace DynamicDataDisplay.Markers
//{
//    public sealed class MarkerChartLegendItem : NewLegendItem, IDisposable
//    {
//        private NewLegend legend;
//        private DevMarkerChart chart;
//        public MarkerChartLegendItem(DevMarkerChart chart, NewLegend legend)
//        {
//            if (chart == null)
//                throw new ArgumentNullException("chart");
//            if (legend == null)
//                throw new ArgumentNullException("legend");

//            this.legend = legend;
//            this.chart = chart;

//            //legend.AddLegendItem(chart, this);

//            //NewLegend.SetChart(this, chart);
//            DataContext = chart;

//            chart.AddHandler(Plotter.PlotterChangedEvent, new PlotterChangedEventHandler(OnPlotterChanged));
//            chart.MarkerBuilderChanged += chart_MarkerBuilderChanged;

//            UpdateVisualContent();
//        }

//        private void OnPlotterChanged(object sender, PlotterChangedEventArgs e)
//        {
//            ChartPlotter plotter = e.CurrentPlotter as ChartPlotter;
//            if (plotter != null)
//            {
//                //plotter.NewLegend.AddLegendItem(chart, this);
//            }
//        }

//        private void chart_MarkerBuilderChanged(object sender, ValueChangedEventArgs<MarkerGenerator> e)
//        {
//            if (e.PreviousValue != null)
//            {
//                e.PreviousValue.Initialized -= markerBuilder_Initialized;
//            }

//            UpdateVisualContent();
//        }

//        private void UpdateVisualContent()
//        {
//            var markerBuilder = chart.MarkerBuilder;
//            if (markerBuilder == null) return;

//            if (!markerBuilder.IsInitialized)
//            {
//                markerBuilder.Initialized += markerBuilder_Initialized;
//                return;
//            }

//            var visualContent = NewLegend.GetVisualContent(chart);
//            if (visualContent != null)
//            {
//                VisualContent = visualContent;
//                return;
//            }

//            if (!markerBuilder.IsReady)
//                return;

//            var sampleData = NewLegend.GetSampleData(chart);
//            var sampleMarker = markerBuilder.CreateMarker(sampleData);
//            VisualContent = sampleMarker;
//        }

//        void markerBuilder_Loaded(object sender, RoutedEventArgs e)
//        {
//            MarkerGenerator generator = (MarkerGenerator)sender;
//            generator.Loaded -= markerBuilder_Loaded;

//            UpdateVisualContent();
//        }

//        private void markerBuilder_Initialized(object sender, EventArgs e)
//        {
//            MarkerGenerator generator = (MarkerGenerator)sender;
//            generator.Initialized -= markerBuilder_Initialized;

//            UpdateVisualContent();
//        }

//        #region IDisposable Members

//        public void Dispose()
//        {
//            var markerBuilder = chart.MarkerBuilder;
//            if (markerBuilder != null)
//            {
//                markerBuilder.Initialized -= markerBuilder_Initialized;
//            }

//            chart.MarkerBuilderChanged -= chart_MarkerBuilderChanged;
//            chart.RemoveHandler(Plotter.PlotterChangedEvent, new EventHandler<PlotterChangedEventArgs>(OnPlotterChanged));

//            legend.RemoveChart(chart);

//            this.chart = null;
//            this.legend = null;
//        }

//        #endregion
//    }
//}
