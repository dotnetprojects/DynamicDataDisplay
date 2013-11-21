using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	/// <summary>
	/// Represents a custom Panel, which performs Arrange of its children independently, and does not remeasure or rearrange itself or all children when one child is
	/// added or removed.
	/// Is intended to be a base class for special layout panels, in which each childr is arranged independently from each other child,
	/// e.g. panel with child's position viewport bound to a rectangle in viewport coordinates.
	/// </summary>
	public abstract class IndividualArrangePanel : Panel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IndependentArrangePanel"/> class.
		/// </summary>
		protected IndividualArrangePanel() { }

		private UIChildrenCollection children;
		/// <summary>
		/// Creates a new <see cref="T:System.Windows.Controls.UIElementCollection"/>.
		/// </summary>
		/// <param name="logicalParent">The logical parent element of the collection to be created.</param>
		/// <returns>
		/// An ordered collection of elements that have the specified logical parent.
		/// </returns>
		protected sealed override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
		{
			children = new UIChildrenCollection(this, logicalParent);
			return children;
		}

		internal bool InBatchAdd
		{
			get { return children.IsAddingMany; }
		}

		internal virtual void BeginBatchAdd()
		{
			children.IsAddingMany = true;
		}

		internal virtual void EndBatchAdd()
		{
			children.IsAddingMany = false;
		}

		/// <summary>
		/// Called when child is added.
		/// </summary>
		/// <param name="child">The added child.</param>
		protected internal virtual void OnChildAdded(FrameworkElement child) { }

		#region Overrides

		/// <summary>
		/// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
		/// </summary>
		/// <param name="index">The zero-based index of the requested child element in the collection.</param>
		/// <returns>
		/// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
		/// </returns>
		protected sealed override Visual GetVisualChild(int index)
		{
			return Children[index];
		}

		/// <summary>
		/// Gets the number of visual child elements within this element.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The number of visual child elements for this element.
		/// </returns>
		protected sealed override int VisualChildrenCount
		{
			get { return Children.Count; }
		}

		/// <summary>
		/// Gets an enumerator for logical child elements of this element.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// An enumerator for logical child elements of this element.
		/// </returns>
		protected sealed override IEnumerator LogicalChildren
		{
			get
			{
				return Children.GetEnumerator();
			}
		}

		#endregion

		internal Vector InternalVisualOffset
		{
			get { return VisualOffset; }
			set { VisualOffset = value; }
		}
	}
}
