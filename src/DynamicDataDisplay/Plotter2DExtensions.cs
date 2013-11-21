using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.Filters;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Extensions for <see cref="Plotter2D"/> - simplified methods to add line and marker charts.
	/// </summary>
	public static class Plotter2DExtensions
	{
		#region Line graphs

		/// <summary>Adds one dimensional graph with random color of line.</summary>
		/// <param name="pointSource">The point source.</param>
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource)
		{
			return AddLineGraph(plotter, pointSource, ColorHelper.CreateRandomHsbColor());
		}

		/// <summary>
		/// Adds one dimensional graph with specified color of line.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineColor">Color of the line.</param>
		/// <returns></returns>
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, Color lineColor)
		{
			return AddLineGraph(plotter, pointSource, lineColor, 1);
		}

		/// <summary>
		/// Adds one dimensional graph with random color if line.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineThickness">The line thickness.</param>
		/// <returns></returns>
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, double lineThickness)
		{
			return AddLineGraph(plotter, pointSource, ColorHelper.CreateRandomHsbColor(), lineThickness);
		}

		/// <summary>
		/// Adds one dimensional graph.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineColor">Color of the line.</param>
		/// <param name="lineThickness">The line thickness.</param>
		/// <param name="description">Description of data</param>
		/// <returns></returns>
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, Color lineColor, double lineThickness,
			string description)
		{
			return AddLineGraph(plotter, pointSource, new Pen(new SolidColorBrush(lineColor), lineThickness), new PenDescription(description));
		}

		/// <summary>
		/// Adds one dimensional graph.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineColor">Color of the line.</param>
		/// <param name="lineThickness">The line thickness.</param>
		/// <returns></returns>
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, Color lineColor, double lineThickness)
		{
			return AddLineGraph(plotter, pointSource, new Pen(new SolidColorBrush(lineColor), lineThickness), null);
		}


		/// <summary>
		/// Adds one dimensional graph.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="description">The description.</param>
		/// <returns></returns>
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, string description)
		{
			LineGraph graph = AddLineGraph(plotter, pointSource);
			graph.Description = new PenDescription(description);
			NewLegend.SetDescription(graph, description);
			return graph;
		}

		/// <summary>
		/// Adds one dimensional graph.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineThickness">The line thickness.</param>
		/// <param name="description">The description.</param>
		/// <returns></returns>
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, double lineThickness, string description)
		{
			var res = AddLineGraph(plotter, pointSource,
				new Pen(new SolidColorBrush(ColorHelper.CreateRandomHsbColor()), lineThickness),
				(PointMarker)null,
				new PenDescription(description));

			return res.LineGraph;
		}

		/// <summary>Adds one dimensional graph to plotter. This method allows you to specify
		/// as much graph parameters as possible</summary>
		/// <param name="pointSource">Source of points to plot</param>
		/// <param name="linePen">Pen to draw the line. If pen is null no lines will be drawn</param>
		/// <param name="marker">Marker to draw on points. If marker is null no points will be drawn</param>
		/// <param name="description">Description of graph to put in legend</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static LineAndMarker<MarkerPointsGraph> AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource,
				Pen linePen, PointMarker marker, Description description)
		{
			if (pointSource == null)
				throw new ArgumentNullException("pointSource");

			var res = new LineAndMarker<MarkerPointsGraph>();

			if (linePen != null) // We are requested to draw line graphs
			{
				LineGraph graph = new LineGraph
				{
					DataSource = pointSource,
					LinePen = linePen
				};
				if (description != null)
				{
					NewLegend.SetDescription(graph, description.Brief);
					graph.Description = description;
				}
				if (marker == null)
				{
					// Add inclination filter only to graphs without markers
					// graph.Filters.Add(new InclinationFilter());
				}

				res.LineGraph = graph;

				graph.Filters.Add(new FrequencyFilter());
				plotter.Children.Add(graph);
			}

			if (marker != null) // We are requested to draw marker graphs
			{
				MarkerPointsGraph markerGraph = new MarkerPointsGraph
				{
					DataSource = pointSource,
					Marker = marker
				};

				res.MarkerGraph = markerGraph;

				plotter.Children.Add(markerGraph);
			}

			return res;
		}

		/// <summary>Adds one dimensional graph to plotter. This method allows you to specify
		/// as much graph parameters as possible</summary>
		/// <param name="pointSource">Source of points to plot</param>
		/// <param name="linePen">Pen to draw the line. If pen is null no lines will be drawn</param>
		/// <param name="marker">Marker to draw on points. If marker is null no points will be drawn</param>
		/// <param name="description">Description of graph to put in legend</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static LineAndMarker<ElementMarkerPointsGraph> AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource,
				Pen linePen, ElementPointMarker marker, Description description)
		{
			if (pointSource == null)
				throw new ArgumentNullException("pointSource");

			var res = new LineAndMarker<ElementMarkerPointsGraph>();



			if (linePen != null) // We are requested to draw line graphs
			{
				LineGraph graph = new LineGraph
				{
					DataSource = pointSource,
					LinePen = linePen
				};
				if (description != null)
				{
					NewLegend.SetDescription(graph, description.Brief);
					graph.Description = description;
				}
				if (marker == null)
				{
					// Add inclination filter only to graphs without markers
					// graph.Filters.Add(new InclinationFilter());
				}

				graph.Filters.Add(new FrequencyFilter());

				res.LineGraph = graph;

				plotter.Children.Add(graph);
			}

			if (marker != null) // We are requested to draw marker graphs
			{
				ElementMarkerPointsGraph markerGraph = new ElementMarkerPointsGraph
				{
					DataSource = pointSource,
					Marker = marker
				};

				res.MarkerGraph = markerGraph;

				plotter.Children.Add(markerGraph);
			}

			return res;
		}

		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, Pen linePen, Description description)
		{
			if (pointSource == null)
				throw new ArgumentNullException("pointSource");
			if (linePen == null)
				throw new ArgumentNullException("linePen");

			LineGraph graph = new LineGraph
			{
				DataSource = pointSource,
				LinePen = linePen
			};
			if (description != null)
			{
				NewLegend.SetDescription(graph, description.Brief);
				graph.Description = description;
			}
			// graph.Filters.Add(new InclinationFilter());
			graph.Filters.Add(new FrequencyFilter());
			plotter.Children.Add(graph);
			return graph;
		}

		#endregion
	}
}
