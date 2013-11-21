using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers.Samples
{
	class Sample1
	{
		void Line1()
		{
			Point[] pts = LoadPoints();
			var ds = DataSource.CreateFrom(pts);

			LineChart chart = new LineChart();
			chart.Stroke = Brushes.Red;
			chart.DataSource = ds;
		}

		void Line2()
		{
			double[] xs = LoadXs();
			double[] ys = LoadYs();

			var ds = DataSource.Union(xs, ys);
		}

		void Line3()
		{
			Transaction[] transactions = LoadTransactions();

			DateTimeAxis dateTimeAxis = new DateTimeAxis();
			plotter.Add(dateTimeAxis);

			lineChart.DataSource = transactions;
			lineChart.XMapping = "Date";
			lineChart.XAxis = dateTimeAxis;
			lineChart.YMapping = "Value";
		}

		void Line4()
		{
			StockInfo[] stocks = LoadStocks();

			// in both cases XAxis is a first DateTimeAxis of Plotter, if such exists, 
			// and exceptions is thrown otherwise.
			lineChart1.DataSource = stocks;
			lineChart1.XMapping = "Time";
			lineChart1.YMapping = "Open";

			lineChart2.DataSource = stocks;
			lineChart2.XMapping = "Time";
			lineChart2.YMapping = "Close";

			// if there are more DateTimeAxes added in plotter and we want to select not the selected by default,
			// we should specify its instance or name (?)
		}

		void Lines4_1()
		{
			StockInfo[] stocks = Load();
			
			chart1.DataSource = stocks;

			DataMapping<StockInfo> mapping = new DataMapping();
			mapping.XMapping = stock => stock.Time;
			mapping.XAxis = dateTimeAxis;
			mapping.YAxis = stock => stock.Open;

			chart1.DataMapping = mapping;
		}

		void Line5()
		{
			Point[] pts = LoadData();

			var ds = DataSource.CreateFrom(pts);

			line.DataSource = pts;
			// should we be able to set DataSource both to pts and ds?
			// should we internally create a DataSource instance based on some data attributes and the be able to refer to this created dataSource outside?

			pts[2] = new Point();
			pts[4].X += 10;

			var ds = DataSource.GetDefaultDataSource(pts);

			// 1
			ds.ReportUpdatedIndex(index = 2);
			ds.ReportUpdatedIndex(index = 4);

			// or 2
			ds.ReportUpdatedRegion(startIndex = 2, count = 2);

			// and line chart should repaint itself.

		}
	}
}
