using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	public abstract class GenericValueSelector<T> : SelectorPlotter, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets or sets the selected value.
		/// </summary>
		/// <value>The selected value.</value>
		public T SelectedValue
		{
			get
			{
				return ValueConversion.ConvertFromDouble(Marker.Position.X);
			}
			set
			{
				double x = ValueConversion.ConvertToDouble(value);
				Point position = new Point(x, 1);

				Marker.Position = position;

				RaiseSelectedValueChanged();
			}
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			Marker.SetBinding(Control.ToolTipProperty, new Binding { Source = this, Path = new PropertyPath("SelectedValue") });
		}

		private void RaiseSelectedValueChanged()
		{
			SelectedValueChanged.Raise(this);
			RaisePropertyChanged("SelectedValue");
		}

		/// <summary>
		/// Occurs when selected value changes.
		/// </summary>
		public event EventHandler SelectedValueChanged;

		#region INotifyPropertyChanged Members

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged.Raise(this, propertyName);
		}
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Gets or sets the minimal and maximal acceptable value of selected values.
		/// </summary>
		/// <value>The range.</value>
		public Range<T> Range
		{
			get
			{
				return new Range<T>();
			}
			set
			{
				double xMin = ValueConversion.ConvertToDouble(value.Min);
				double xMax = ValueConversion.ConvertToDouble(value.Max);

				DataRect domain = DataRect.Create(xMin, 0, xMax, 1);
				Viewport.Visible = domain;

				Domain = new Range<double>(xMin, xMax);
				Marker.InvalidateProperty(DraggablePoint.PositionProperty);
			}
		}

		protected IValueConversion<T> ValueConversion { get; set; }

		internal AxisBase<T> Axis { get { return (AxisBase<T>)ValueConversion; } }

		protected override void OnMarkerPositionChanged(PositionChangedEventArgs e)
		{
			RaiseSelectedValueChanged();
		}
	}
}
