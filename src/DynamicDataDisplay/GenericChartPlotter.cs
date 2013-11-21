using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay
{
	public sealed class GenericChartPlotter<THorizontal, TVertical>
	{
		private readonly AxisBase<THorizontal> horizontalAxis;
		public AxisBase<THorizontal> HorizontalAxis
		{
			get { return horizontalAxis; }
		}

		private readonly AxisBase<TVertical> verticalAxis;
		public AxisBase<TVertical> VerticalAxis
		{
			get { return verticalAxis; }
		}

		private readonly ChartPlotter plotter;
		public ChartPlotter Plotter
		{
			get { return plotter; }
		}

		public Func<THorizontal, double> HorizontalToDoubleConverter
		{
			get { return horizontalAxis.ConvertToDouble; }
		}

		public Func<double, THorizontal> DoubleToHorizontalConverter
		{
			get { return horizontalAxis.ConvertFromDouble; }
		}

		public Func<TVertical, double> VerticalToDoubleConverter
		{
			get { return verticalAxis.ConvertToDouble; }
		}

		public Func<double, TVertical> DoubleToVerticalConverter
		{
			get { return verticalAxis.ConvertFromDouble; }
		}

		internal GenericChartPlotter(ChartPlotter plotter) : this(plotter, plotter.MainHorizontalAxis as AxisBase<THorizontal>, plotter.MainVerticalAxis as AxisBase<TVertical>) { }

		internal GenericChartPlotter(ChartPlotter plotter, AxisBase<THorizontal> horizontalAxis, AxisBase<TVertical> verticalAxis)
		{
			if (horizontalAxis == null)
				throw new ArgumentNullException(Strings.Exceptions.PlotterMainHorizontalAxisShouldNotBeNull);
			if (verticalAxis == null)
				throw new ArgumentNullException(Strings.Exceptions.PlotterMainVerticalAxisShouldNotBeNull);

			this.horizontalAxis = horizontalAxis;
			this.verticalAxis = verticalAxis;

			this.plotter = plotter;
		}

		public GenericRect<THorizontal, TVertical> ViewportRect
		{
			get
			{
				DataRect viewportRect = plotter.Viewport.Visible;
				return CreateGenericRect(viewportRect);
			}
			set
			{
				DataRect viewportRect = CreateRect(value);
				plotter.Viewport.Visible = viewportRect;
			}
		}

		public GenericRect<THorizontal, TVertical> DataRect
		{
			get
			{
				DataRect dataRect = plotter.Viewport.Visible.ViewportToData(plotter.Viewport.Transform);
				return CreateGenericRect(dataRect);
			}
			set
			{
				DataRect dataRect = CreateRect(value);
				plotter.Viewport.Visible = dataRect.DataToViewport(plotter.Viewport.Transform);
			}
		}

		private DataRect CreateRect(GenericRect<THorizontal, TVertical> value)
		{
			double xMin = HorizontalToDoubleConverter(value.XMin);
			double xMax = HorizontalToDoubleConverter(value.XMax);
			double yMin = VerticalToDoubleConverter(value.YMin);
			double yMax = VerticalToDoubleConverter(value.YMax);

			return new DataRect(new Point(xMin, yMin), new Point(xMax, yMax));
		}

		private GenericRect<THorizontal, TVertical> CreateGenericRect(DataRect rect)
		{
			double xMin = rect.XMin;
			double xMax = rect.XMax;
			double yMin = rect.YMin;
			double yMax = rect.YMax;

			GenericRect<THorizontal, TVertical> res = new GenericRect<THorizontal, TVertical>(
				DoubleToHorizontalConverter(xMin),
				DoubleToVerticalConverter(yMin),
				DoubleToHorizontalConverter(xMax),
				DoubleToVerticalConverter(yMax));

			return res;
		}

	}
}
