using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.VisualStudio.Design
{
	public class DraggablePointAdornerProvider : PrimarySelectionAdornerProvider
	{

		private ModelItem adornedControlModel = null;
		private DraggablePoint draggablePoint = new DraggablePoint();

		public DraggablePointAdornerProvider()
		{
		}

		protected override void Activate(ModelItem item, DependencyObject view)
		{
			view.SetValue(Control.BackgroundProperty, Brushes.Aqua);

			adornedControlModel = item;
			adornedControlModel.PropertyChanged += adornedControlModel_PropertyChanged;

			AdornerPanel draggablePointAdornerPanel = new AdornerPanel();

			AdornerPlacementCollection placement = new AdornerPlacementCollection();

			draggablePointAdornerPanel.CoordinateSpace = AdornerCoordinateSpaces.Layout;

			base.Activate(item, view);
		}

		void adornedControlModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Debug.WriteLine("Property \"" + e.PropertyName + "\" changed.");
		}
	}
}
