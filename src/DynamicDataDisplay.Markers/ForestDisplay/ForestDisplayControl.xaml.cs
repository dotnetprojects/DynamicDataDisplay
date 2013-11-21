using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DynamicDataDisplay.Markers.ForestDisplay;
using DynamicDataDisplay.Markers.MarkerGenerators;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Markers
{
	/// <summary>
	/// Represents a control for showing forest growth dynamics.
	/// </summary>
	public partial class ForestDisplayControl : UserControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ForestDisplayControl"/> class.
		/// </summary>
		public ForestDisplayControl()
		{
			InitializeComponent();

			plotter.Viewport.Restrictions.Add(new PhysicalProportionsRestriction(1.0));

			plotter.Viewport.ContentBoundsChanged += Viewport_ContentBoundsChanged;
		}

		private void Viewport_ContentBoundsChanged(object sender, EventArgs e)
		{
			plotter.Viewport.Domain = plotter.Viewport.UnitedContentBounds.Zoom(plotter.Viewport.UnitedContentBounds.GetCenter(), 1.05);
		}

		#region Properties

		private Dictionary<string, TreeSpeciesInfo> speciesMappings;

		/// <summary>
		/// Gets or sets the species mappings.
		/// </summary>
		/// <value>The species mappings.</value>
		public Dictionary<string, TreeSpeciesInfo> SpeciesMappings
		{
			get { return speciesMappings; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("SpeciesMappings");

				if (speciesMappings != value)
				{
					speciesMappings = value;
					var converter = new ForestConverter(value);
					forestDisplayGenerator.ForestConverter = converter;
					OnForestConverterChanged(converter);
				}
			}
		}

		private void OnForestConverterChanged(ForestConverter forestConverter)
		{
			Dictionary<string, TreeSpeciesInfo> dict = forestConverter.Mappings;

			foreach (var item in dict)
			{
				var geometry = (Geometry) forestDisplayGenerator.Resources[item.Value.ViewID];
				var crown = new Path
				            	{
				            		Width = 15,
				            		Height = 15,
				            		Data = geometry,
				            		Stretch = Stretch.Fill,
				            		Fill = (Brush) forestConverter.Convert(item.Key, typeof (Brush), null, null)
				            	};
				//plotter.NewLegend.AddLegendItem(markerChart, new NewLegendItem {Description = item.Key, VisualContent = crown});
			}
		}

		#endregion // end of Properties
	}
}