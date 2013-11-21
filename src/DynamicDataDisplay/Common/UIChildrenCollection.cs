using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	/// <summary>
	/// Collection of UI children of <see cref="IndependentArrangePanel"/>.
	/// </summary>
	internal sealed class UIChildrenCollection : UIElementCollection
	{
		IndividualArrangePanel hostPanel;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIChildrenCollection"/> class.
		/// </summary>
		internal UIChildrenCollection(UIElement visualParent, FrameworkElement logicalParent)
			: base(visualParent, logicalParent)
		{
			hostPanel = (IndividualArrangePanel)visualParent;
			visualChildren = new VisualCollection(visualParent);
		}

		private readonly VisualCollection visualChildren;
		private bool isAddingMany = false;
		public bool IsAddingMany
		{
			get { return isAddingMany; }
			set { isAddingMany = value; }
		}

		public override int Add(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			SetLogicalParent(element);

			var index = visualChildren.Add(element);

			if (isAddingMany)
			{
				hostPanel.InvalidateMeasure();
			}
			else
			{
				hostPanel.OnChildAdded((FrameworkElement)element);
			}

			return index;
		}

		public override void Clear()
		{
			if (visualChildren.Count > 0)
			{
				Visual[] visualArray = new Visual[visualChildren.Count];
				for (int i = 0; i < visualChildren.Count; i++)
				{
					visualArray[i] = visualChildren[i];
				}

				visualChildren.Clear();

				for (int i = 0; i < visualArray.Length; i++)
				{
					UIElement element = visualArray[i] as UIElement;
					if (element != null)
					{
						ClearLogicalParent(element);
					}
				}
			}
		}

		public override bool Contains(UIElement element)
		{
			return visualChildren.Contains(element);
		}

		public override void CopyTo(Array array, int index)
		{
			visualChildren.CopyTo(array, index);
		}

		public override void CopyTo(UIElement[] array, int index)
		{
			visualChildren.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return visualChildren.GetEnumerator();
		}

		public override int IndexOf(UIElement element)
		{
			return visualChildren.IndexOf(element);
		}

		public override void Insert(int index, UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			hostPanel.OnChildAdded((FrameworkElement)element);
			SetLogicalParent(element);
			visualChildren.Insert(index, element);
		}

		public override void Remove(UIElement element)
		{
			visualChildren.Remove(element);
			ClearLogicalParent(element);
		}

		public override void RemoveAt(int index)
		{
			UIElement element = visualChildren[index] as UIElement;
			visualChildren.RemoveAt(index);
			if (element != null)
			{
				ClearLogicalParent(element);
			}
		}

		public override void RemoveRange(int index, int count)
		{
			int actualCount = visualChildren.Count;
			if (count > (actualCount - index))
			{
				count = actualCount - index;
			}

			if (count > 0)
			{
				Visual[] visualArray = new Visual[count];
				int copyIndex = index;
				for (int i = 0; i < count; i++)
				{
					visualArray[i] = visualChildren[copyIndex];
					copyIndex++;
				}

				visualChildren.RemoveRange(index, count);

				for (int i = 0; i < count; i++)
				{
					UIElement element = visualArray[i] as UIElement;
					if (element != null)
					{
						ClearLogicalParent(element);
					}
				}
			}
		}

		public override UIElement this[int index]
		{
			get
			{
				return visualChildren[index] as UIElement;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (visualChildren[index] != value)
				{
					UIElement element = visualChildren[index] as UIElement;
					if (element != null)
					{
						ClearLogicalParent(element);
					}

					hostPanel.OnChildAdded((FrameworkElement)value);
					visualChildren[index] = value;
					SetLogicalParent(value);
				}
			}
		}

		public override int Capacity
		{
			get { return visualChildren.Capacity; }
			set { visualChildren.Capacity = value; }
		}

		public override int Count
		{
			get { return visualChildren.Count; }
		}

		public override bool IsSynchronized
		{
			get { return visualChildren.IsSynchronized; }
		}

		public override object SyncRoot
		{
			get { return visualChildren.SyncRoot; }
		}
	}
}
