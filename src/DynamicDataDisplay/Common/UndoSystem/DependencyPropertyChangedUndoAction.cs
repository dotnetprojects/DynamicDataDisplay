using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.UndoSystem
{
	public class DependencyPropertyChangedUndoAction : UndoAction
	{
		private readonly DependencyProperty property;
		private readonly DependencyObject target;
		private readonly object oldValue;
		private readonly object newValue;

		public DependencyPropertyChangedUndoAction(DependencyObject target, DependencyProperty property, object oldValue, object newValue)
		{
			this.target = target;
			this.property = property;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public DependencyPropertyChangedUndoAction(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			this.target = target;
			this.property = e.Property;
			this.oldValue = e.OldValue;
			this.newValue = e.NewValue;
		}

		public override void Do()
		{
			target.SetValue(property, newValue);
		}

		public override void Undo()
		{
			target.SetValue(property, oldValue);
		}
	}
}
