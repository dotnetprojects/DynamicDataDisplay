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

namespace Microsoft.Research.DynamicDataDisplay
{
    public interface IPlotterElement
    {
        void OnPlotterAttached(Plotter plotter);
        void OnPlotterDetaching(Plotter plotter);
        Plotter Plotter { get; }
    }

    public sealed class PlotterConnectionEventArgs : EventArgs
    {
        public PlotterConnectionEventArgs(Plotter plotter)
        {
            this.plotter = plotter;
        }

        private readonly Plotter plotter;
        public Plotter Plotter
        {
            get { return plotter; }
        }
    }

    public abstract class PlotterElement : ContentControl, IPlotterElement
    {
        private Plotter plotter;
        
        public Plotter Plotter
        {
            get { return plotter; }
        }

        /// <summary>This method is invoked when element is attached to plotter. It is the place
        /// to put additional controls to Plotter</summary>
        /// <param name="plotter">Plotter for this element</param>
        public virtual void OnPlotterAttached(Plotter plotter)
        {
            this.plotter = plotter;
            RaisePlotterAttached(plotter);
        }

        public event EventHandler<PlotterConnectionEventArgs> PlotterAttached;
        protected void RaisePlotterAttached(Plotter plotter)
        {
            if (PlotterAttached != null)
            {
                PlotterAttached(this, new PlotterConnectionEventArgs(plotter));
            }
        }

        /// <summary>This method is invoked when element is being detached from plotter. If additional
        /// controls were put on plotter in OnPlotterAttached method, they should be removed here</summary>
        /// <remarks>This method is always called in pair with OnPlotterAttached</remarks>
        public virtual void OnPlotterDetaching(Plotter plotter)
        {
            RaisePlotterDetaching(plotter);
            this.plotter = null;
        }

        public event EventHandler<PlotterConnectionEventArgs> PlotterDetaching;
        protected void RaisePlotterDetaching(Plotter plotter)
        {
            if (PlotterDetaching != null)
            {
                PlotterDetaching(this, new PlotterConnectionEventArgs(plotter));
            }
        }
    }
}
