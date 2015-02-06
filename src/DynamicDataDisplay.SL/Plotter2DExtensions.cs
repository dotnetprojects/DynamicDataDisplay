using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Research.DynamicDataDisplay
{
    /// <summary>
    /// Extensions for <see cref="Plotter2D"/> - simplified methods to add line and marker charts.
    /// </summary>
    public static class Plotter2DExtensions
    {
        #region Line graphs

        /// <summary>
        /// Adds one dimensional graph.
        /// </summary>
        /// <param name="pointSource">The point source.</param>
        /// <param name="lineColor">Color of the line.</param>
        /// <param name="lineThickness">The line thickness.</param>
        /// <param name="description">Description of data</param>
        /// <returns></returns>
        
        /*[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static LineGraph AddLineGraph(this Plotter2D plotter, IPointDataSource pointSource, Color lineColor, double lineThickness,
            string description)
            {
            if (pointSource == null)
                throw new ArgumentNullException("pointSource");
            if (lineColor == null)
                throw new ArgumentNullException("lineColor");

            LineGraph graph = new LineGraph
            {
                DataSource = pointSource,
                LineColor = lineColor,
                LineThickness = lineThickness
            };
            if (description != null)
            {
                graph.Description = description;
            }
            plotter.Children.Add(graph);
            return graph;
        }*/

        #endregion
    }
}
