using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.IDataSource2D<double>;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
    public abstract class IsolineGraphBase : ContentGraph
    {
        protected IsolineGraphBase() { }

        private IsolineCollection collection = new IsolineCollection();
        protected IsolineCollection Collection
        {
            get { return collection; }
            set { collection = value; }
        }

        private readonly IsolineBuilder isolineBuilder = new IsolineBuilder();
        protected IsolineBuilder IsolineBuilder
        {
            get { return isolineBuilder; }
        }

        private readonly IsolineTextAnnotater annotater = new IsolineTextAnnotater();
        protected IsolineTextAnnotater Annotater
        {
            get { return annotater; }
        }

        #region Properties

		#region IsolineCollection property

		public IsolineCollection IsolineCollection
		{
			get { return (IsolineCollection)GetValue(IsolineCollectionProperty); }
			set { SetValue(IsolineCollectionProperty, value); }
		}

		public static readonly DependencyProperty IsolineCollectionProperty = DependencyProperty.Register(
		  "IsolineCollection",
		  typeof(IsolineCollection),
		  typeof(IsolineGraphBase),
		  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		#endregion // end of IsolineCollection property

		#region WayBeforeTextMultiplier

		public double WayBeforeTextMultiplier
        {
            get { return (double)GetValue(WayBeforeTextMultiplierProperty); }
            set { SetValue(WayBeforeTextMultiplierProperty, value); }
        }

        public static readonly DependencyProperty WayBeforeTextMultiplierProperty = DependencyProperty.Register(
          "WayBeforeTextCoeff",
          typeof(double),
          typeof(IsolineGraphBase),
          new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits, OnIsolinePropertyChanged));

        #endregion // end of WayBeforeTextCoeff

        private static void OnIsolinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // todo do smth here
        }

        #region Palette property

        public IPalette Palette
        {
            get { return (IPalette)GetValue(PaletteProperty); }
            set { SetValue(PaletteProperty, value); }
        }

        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
          "Palette",
          typeof(IPalette),
          typeof(IsolineGraphBase),
          new FrameworkPropertyMetadata(new HSBPalette(), FrameworkPropertyMetadataOptions.Inherits, OnIsolinePropertyChanged), ValidatePalette);

        private static bool ValidatePalette(object value)
        {
            return value != null;
        }

        #endregion // end of Palette property

        #region DataSource property

        public DataSource DataSource
        {
            get { return (DataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
          "DataSource",
          typeof(DataSource),
          typeof(IsolineGraphBase),
          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnDataSourceChanged));

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsolineGraphBase owner = (IsolineGraphBase)d;
            owner.OnDataSourceChanged((DataSource)e.OldValue, (DataSource)e.NewValue);
        }

        protected virtual void OnDataSourceChanged(IDataSource2D<double> prevDataSource, IDataSource2D<double> currDataSource)
        {
            if (prevDataSource != null)
                prevDataSource.Changed -= OnDataSourceChanged;
            if (currDataSource != null)
                currDataSource.Changed += OnDataSourceChanged;

            UpdateDataSource();
            CreateUIRepresentation();

            RaiseEvent(new RoutedEventArgs(BackgroundRenderer.UpdateRequested));
        }

        #endregion // end of DataSource property

		#region DrawLabels property

		public bool DrawLabels
        {
            get { return (bool)GetValue(DrawLabelsProperty); }
            set { SetValue(DrawLabelsProperty, value); }
        }

        public static readonly DependencyProperty DrawLabelsProperty = DependencyProperty.Register(
            "DrawLabels",
            typeof(bool),
            typeof(IsolineGraphBase),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, OnIsolinePropertyChanged));

		#endregion // end of DrawLabels property
		
		#region LabelStringFormat

		public string LabelStringFormat
        {
            get { return (string)GetValue(LabelStringFormatProperty); }
            set { SetValue(LabelStringFormatProperty, value); }
        }

        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register(
          "LabelStringFormat",
          typeof(string),
          typeof(IsolineGraphBase),
          new FrameworkPropertyMetadata("F", FrameworkPropertyMetadataOptions.Inherits, OnIsolinePropertyChanged));

        #endregion // end of LabelStringFormat

		#region UseBezierCurves

		public bool UseBezierCurves
		{
			get { return (bool)GetValue(UseBezierCurvesProperty); }
			set { SetValue(UseBezierCurvesProperty, value); }
		}

		public static readonly DependencyProperty UseBezierCurvesProperty = DependencyProperty.Register(
		  "UseBezierCurves",
		  typeof(bool),
		  typeof(IsolineGraphBase),
		  new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

		#endregion // end of UseBezierCurves

		#endregion // end of Properties

		#region DataSource

		//private DataSource dataSource = null;
        ///// <summary>
        ///// Gets or sets the data source.
        ///// </summary>
        ///// <value>The data source.</value>
        //public DataSource DataSource
        //{
        //    get { return dataSource; }
        //    set
        //    {
        //        if (dataSource != value)
        //        {
        //            DetachDataSource(dataSource);
        //            dataSource = value;
        //            AttachDataSource(dataSource);

        //            UpdateDataSource();
        //        }
        //    }
        //}

        #region MissineValue property

        public double MissingValue
        {
            get { return (double)GetValue(MissingValueProperty); }
            set { SetValue(MissingValueProperty, value); }
        }

        public static readonly DependencyProperty MissingValueProperty = DependencyProperty.Register(
          "MissingValue",
          typeof(double),
          typeof(IsolineGraphBase),
          new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.Inherits, OnMissingValueChanged));

        private static void OnMissingValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsolineGraphBase owner = (IsolineGraphBase)d;
            owner.UpdateDataSource();
        }

        #endregion // end of MissineValue property

        public void SetDataSource(DataSource dataSource, double missingValue)
        {
            DataSource = dataSource;
            MissingValue = missingValue;

            UpdateDataSource();
        }

        /// <summary>
        /// This method is called when data source changes.
        /// </summary>
        protected virtual void UpdateDataSource()
        {
        }

        protected virtual void CreateUIRepresentation() { }

        protected virtual void OnDataSourceChanged(object sender, EventArgs e)
        {
            UpdateDataSource();
        }

        #endregion

        #region StrokeThickness

        /// <summary>
        /// Gets or sets thickness of isoline lines.
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
              "StrokeThickness",
              typeof(double),
              typeof(IsolineGraphBase),
              new FrameworkPropertyMetadata(2.0, OnLineThicknessChanged));

        private static void OnLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsolineGraphBase graph = (IsolineGraphBase)d;
            graph.OnLineThicknessChanged();
        }

        protected virtual void OnLineThicknessChanged() { }

        #endregion
    }
}
