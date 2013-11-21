using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.UndoSystem
{
	public class UndoProvider : INotifyPropertyChanged
	{
		private readonly ActionStack undoStack = new ActionStack();
		private readonly ActionStack redoStack = new ActionStack();

		public UndoProvider()
		{
			undoStack.IsEmptyChanged += OnUndoStackIsEmptyChanged;
			redoStack.IsEmptyChanged += OnRedoStackIsEmptyChanged;
		}

		private bool isEnabled = true;
		public bool IsEnabled
		{
			get { return isEnabled; }
			set { isEnabled = value; }
		}

		private void OnUndoStackIsEmptyChanged(object sender, EventArgs e)
		{
			PropertyChanged.Raise(this, "CanUndo");
		}

		private void OnRedoStackIsEmptyChanged(object sender, EventArgs e)
		{
			PropertyChanged.Raise(this, "CanRedo");
		}

		public void AddAction(UndoAction action)
		{
			if (!isEnabled)
				return;

			if (state != UndoState.None)
				return;

			undoStack.Push(action);
			redoStack.Clear();
		}

		public void Undo()
		{
			var action = undoStack.Pop();
			redoStack.Push(action);

			state = UndoState.Undoing;
			try
			{
				action.Undo();
			}
			finally
			{
				state = UndoState.None;
			}
		}

		public void Redo()
		{
			var action = redoStack.Pop();
			undoStack.Push(action);

			state = UndoState.Redoing;
			try
			{
				action.Do();
			}
			finally
			{
				state = UndoState.None;
			}
		}

		public bool CanUndo
		{
			get { return !undoStack.IsEmpty; }
		}

		public bool CanRedo
		{
			get { return !redoStack.IsEmpty; }
		}

		private UndoState state = UndoState.None;
		public UndoState State
		{
			get { return state; }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private readonly Dictionary<CaptureKeyHolder, object> captureHolders = new Dictionary<CaptureKeyHolder, object>();
		public void CaptureOldValue(DependencyObject target, DependencyProperty property, object oldValue)
		{
			captureHolders[new CaptureKeyHolder { Target = target, Property = property }] = oldValue;
		}

		public void CaptureNewValue(DependencyObject target, DependencyProperty property, object newValue)
		{
			var holder = new CaptureKeyHolder { Target = target, Property = property };
			if (captureHolders.ContainsKey(holder))
			{
				object oldValue = captureHolders[holder];
				captureHolders.Remove(holder);

				if (!Object.Equals(oldValue, newValue))
				{
					var action = new DependencyPropertyChangedUndoAction(target, property, oldValue, newValue);
					AddAction(action);
				}
			}
		}

		private sealed class CaptureKeyHolder
		{
			public DependencyObject Target { get; set; }
			public DependencyProperty Property { get; set; }

			public override int GetHashCode()
			{
				return Target.GetHashCode() ^ Property.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == null) return false;

				CaptureKeyHolder other = obj as CaptureKeyHolder;
				if (other == null) return false;

				return Target == other.Target && Property == other.Property;
			}
		}
	}

	public enum UndoState
	{
		None,
		Undoing,
		Redoing
	}
}
