using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Selectors
{
	/// <summary>
	/// Represents a wrapper around FrameworkElement that has a Click event for given MouseButton.
	/// </summary>
	public sealed class MouseClickWrapper
	{
		private FrameworkElement element;
		private MouseButton mouseButton;
		/// <summary>
		/// Initializes a new instance of the <see cref="MouseClickWrapper"/> class with specified FrameworkElement as mouse events source and specified MouseButton 
		/// to notify about its clicks.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="mouseButton">The mouse button.</param>
		public MouseClickWrapper(FrameworkElement element, MouseButton mouseButton)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			this.element = element;
			this.mouseButton = mouseButton;

			element.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown));
			element.AddHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp));
		}

		/// <summary>
		/// Unsubscribes from mouse events of specified FrameworkElement.
		/// </summary>
		public void Unsubscribe()
		{
			element.RemoveHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown));
			element.RemoveHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp));
		}

		private Point mouseDownPosition;
		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == mouseButton)
			{
				mouseDownPosition = e.GetPosition(element);
			}
		}

		private double maxMouseShiftDistance = 2.0; // px
		/// <summary>
		/// Gets or sets the maximal mouse shift distance.
		/// If mouse shifted on less distance between mouse button down and up events, this will be interpreted as mouse click.
		/// </summary>
		/// <value>The max mouse shift distance.</value>
		public double MaxMouseShiftDistance
		{
			get { return maxMouseShiftDistance; }
			set { maxMouseShiftDistance = value; }
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == mouseButton)
			{
				var mouseUpPosition = e.GetPosition(element);
				if ((mouseUpPosition - mouseDownPosition).Length < maxMouseShiftDistance)
				{
					RaiseClick(e);
				}
			}
		}

		private void RaiseClick(MouseButtonEventArgs args)
		{
			if (Click != null)
				Click(element, args);
		}
		/// <summary>
		/// Occurs when mouse clicks on specified ui element.
		/// </summary>
		public event MouseButtonEventHandler Click;
	}
}
