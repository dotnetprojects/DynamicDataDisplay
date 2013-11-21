using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
	public partial class Viewport2D
	{
		#region IsContentBoundsHost attached property

		public static bool GetIsContentBoundsHost(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsContentBoundsHostProperty);
		}

		public static void SetIsContentBoundsHost(DependencyObject obj, bool value)
		{
			obj.SetValue(IsContentBoundsHostProperty, value);
		}

		public static readonly DependencyProperty IsContentBoundsHostProperty = DependencyProperty.RegisterAttached(
		  "IsContentBoundsHost",
		  typeof(bool),
		  typeof(Viewport2D),
		  new FrameworkPropertyMetadata(true, OnIsContentBoundsChanged));

		private static void OnIsContentBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			IPlotterElement plotterElement = d as IPlotterElement;
			if (plotterElement != null && plotterElement.Plotter != null)
			{
				Plotter2D plotter2d = (Plotter2D)plotterElement.Plotter;
				plotter2d.Viewport.UpdateContentBoundsHosts();
			}
		}

		#endregion

		#region ContentBounds attached property

		public static DataRect GetContentBounds(DependencyObject obj)
		{
			return (DataRect)obj.GetValue(ContentBoundsProperty);
		}

		public static void SetContentBounds(DependencyObject obj, DataRect value)
		{
			obj.SetValue(ContentBoundsProperty, value);
		}

		public static readonly DependencyProperty ContentBoundsProperty = DependencyProperty.RegisterAttached(
		  "ContentBounds",
		  typeof(DataRect),
		  typeof(Viewport2D),
		  new FrameworkPropertyMetadata(DataRect.Empty, OnContentBoundsChanged, CoerceContentBounds));

		private static object CoerceContentBounds(DependencyObject d, object value)
		{
			DataRect prevBounds = GetContentBounds(d);
			DataRect currBounds = (DataRect)value;

			bool approximateComparanceAllowed = GetUsesApproximateContentBoundsComparison(d);

			bool areClose = approximateComparanceAllowed && currBounds.IsCloseTo(prevBounds, 0.005);
			if (areClose)
				return DependencyProperty.UnsetValue;
			else
				return value;
		}

		private static void OnContentBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			IPlotterElement element = d as IPlotterElement;
			if (element != null)
			{
				FrameworkElement frElement = element as FrameworkElement;
				if (frElement != null)
				{
					frElement.RaiseEvent(new RoutedEventArgs(ContentBoundsChangedEvent));
				}

				Plotter2D plotter2d = element.Plotter as Plotter2D;
				if (plotter2d != null)
				{
                    plotter2d.Viewport.UpdateContentBoundsHosts();
				}
			}
		}

		public static readonly RoutedEvent ContentBoundsChangedEvent = EventManager.RegisterRoutedEvent(
			"ContentBoundsChanged",
			RoutingStrategy.Direct,
			typeof(RoutedEventHandler),
			typeof(Viewport2D));

		#endregion

		#region UsesApproximateContentBoundsComparison

		/// <summary>
		/// Gets a value indicating whether approximate content bounds comparison will be used while deciding whether to updating Viewport2D.ContentBounds
		/// attached dependency property or not.
		/// Approximate content bounds comparison can make Viewport's Visible to changed less frequent, but it can lead to
		/// some bugs if content bounds are large but visible area is not compared to them.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if approximate content bounds comparison is used while deciding whether to set new value of content bounds, or not; otherwise, <c>false</c>.
		/// </value>
		public static bool GetUsesApproximateContentBoundsComparison(DependencyObject obj)
		{
			return (bool)obj.GetValue(UsesApproximateContentBoundsComparisonProperty);
		}

		/// <summary>
		/// Sets a value indicating whether approximate content bounds comparison will be used while deciding whether to updating Viewport2D.ContentBounds
		/// attached dependency property or not.
		/// Approximate content bounds comparison can make Viewport's Visible to changed less frequent, but it can lead to
		/// some bugs if content bounds are large but visible area is not compared to them.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if approximate content bounds comparison is used while deciding whether to set new value of content bounds, or not; otherwise, <c>false</c>.
		/// </value>		
		public static void SetUsesApproximateContentBoundsComparison(DependencyObject obj, bool value)
		{
			obj.SetValue(UsesApproximateContentBoundsComparisonProperty, value);
		}

		public static readonly DependencyProperty UsesApproximateContentBoundsComparisonProperty = DependencyProperty.RegisterAttached(
		  "UsesApproximateContentBoundsComparison",
		  typeof(bool),
		  typeof(Viewport2D),
		  new FrameworkPropertyMetadata(true, OnUsesApproximateContentBoundsComparisonChanged));

		private static void OnUsesApproximateContentBoundsComparisonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			IPlotterElement element = d as IPlotterElement;
			if (element != null)
			{
				Plotter2D plotter2d = element.Plotter as Plotter2D;
				if (plotter2d != null)
				{
					plotter2d.Viewport.UpdateVisible();
				}
			}
		}

		#endregion // end of UsesApproximateContentBoundsComparison

		#region UseDeferredPanning attached property

		public static bool GetUseDeferredPanning(DependencyObject obj)
		{
			return (bool)obj.GetValue(UseDeferredPanningProperty);
		}

		public static void SetUseDeferredPanning(DependencyObject obj, bool value)
		{
			obj.SetValue(UseDeferredPanningProperty, value);
		}

		public static readonly DependencyProperty UseDeferredPanningProperty = DependencyProperty.RegisterAttached(
		  "UseDeferredPanning",
		  typeof(bool),
		  typeof(Viewport2D),
		  new FrameworkPropertyMetadata(false));

		#endregion // end of UseDeferredPanning attached property
	}
}
