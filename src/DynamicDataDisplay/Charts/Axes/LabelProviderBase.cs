using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
	/// <summary>
	/// Contains data for custom generation of tick's label.
	/// </summary>
	/// <typeparam name="T">Type of ticks</typeparam>
	public sealed class LabelTickInfo<T>
	{
		internal LabelTickInfo() { }

		/// <summary>
		/// Gets or sets the tick.
		/// </summary>
		/// <value>The tick.</value>
		public T Tick { get; internal set; }
		/// <summary>
		/// Gets or sets additional info about ticks range.
		/// </summary>
		/// <value>The info.</value>
		public object Info { get; internal set; }
		/// <summary>
		/// Gets or sets the index of tick in ticks array.
		/// </summary>
		/// <value>The index.</value>
		public int Index { get; internal set; }
	}

	/// <summary>
	/// Base class for all label providers.
	/// Contains a number of properties that can be used to adjust generated labels.
	/// </summary>
	/// <typeparam name="T">Type of ticks, which labels are generated for</typeparam>
	/// <remarks>
	/// Order of apllication of custom label string properties:
	/// If CustomFormatter is not null, it is called first.
	/// Then, if it was null or if it returned null string,
	/// virtual GetStringCore method is called. It can be overloaded in subclasses. GetStringCore should not return null.
	/// Then if LabelStringFormat is not null, it is applied.
	/// After label's UI was created, you can change it by setting CustomView delegate - it allows you to adjust 
	/// UI properties of label. Note: not all labelProviders takes CustomView into account.
	/// </remarks>
	public abstract class LabelProviderBase<T>
	{

		#region Private

		private string labelStringFormat = null;
		private Func<LabelTickInfo<T>, string> customFormatter = null;
		private Action<LabelTickInfo<T>, UIElement> customView = null;

		#endregion

		private static readonly UIElement[] emptyLabelsArray = new UIElement[0];
		protected static UIElement[] EmptyLabelsArray
		{
			get { return emptyLabelsArray; }
		}

		/// <summary>
		/// Creates labels by given ticks info.
		/// Is not intended to be called from your code.
		/// </summary>
		/// <param name="ticksInfo">The ticks info.</param>
		/// <returns>Array of <see cref="UIElement"/>s, which are axis labels for specified axis ticks.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract UIElement[] CreateLabels(ITicksInfo<T> ticksInfo);

		/// <summary>
		/// Gets or sets the label string format.
		/// </summary>
		/// <value>The label string format.</value>
		public string LabelStringFormat
		{
			get { return labelStringFormat; }
			set
			{
				if (labelStringFormat != value)
				{
					labelStringFormat = value;
					RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the custom formatter - delegate that can be called to create custom string representation of tick.
		/// </summary>
		/// <value>The custom formatter.</value>
		public Func<LabelTickInfo<T>, string> CustomFormatter
		{
			get { return customFormatter; }
			set
			{
				if (customFormatter != value)
				{
					customFormatter = value;
					RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the custom view - delegate that is used to create a custom, non-default look of axis label.
		/// Can be used to adjust some UI properties of generated label.
		/// </summary>
		/// <value>The custom view.</value>
		public Action<LabelTickInfo<T>, UIElement> CustomView
		{
			get { return customView; }
			set
			{
				if (customView != value)
				{
					customView = value;
					RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Sets the custom formatter.
		/// This is alternative to CustomFormatter property setter, the only difference is that Visual Studio shows 
		/// more convenient tooltip for methods rather than for properties' setters.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		public void SetCustomFormatter(Func<LabelTickInfo<T>, string> formatter)
		{
			CustomFormatter = formatter;
		}

		/// <summary>
		/// Sets the custom view.
		/// This is alternative to CustomView property setter, the only difference is that Visual Studio shows 
		/// more convenient tooltip for methods rather than for properties' setters.
		/// </summary>
		/// <param name="view">The view.</param>
		public void SetCustomView(Action<LabelTickInfo<T>, UIElement> view)
		{
			CustomView = view;
		}

		protected virtual string GetString(LabelTickInfo<T> tickInfo)
		{
			string text = null;
			if (CustomFormatter != null)
			{
				text = CustomFormatter(tickInfo);
			}
			if (text == null)
			{
				text = GetStringCore(tickInfo);

				if (text == null)
					throw new ArgumentNullException(Strings.Exceptions.TextOfTickShouldNotBeNull);
			}
			if (LabelStringFormat != null)
			{
				text = String.Format(LabelStringFormat, text);
			}

			return text;
		}

		protected virtual string GetStringCore(LabelTickInfo<T> tickInfo)
		{
			return tickInfo.Tick.ToString();
		}

		protected void ApplyCustomView(LabelTickInfo<T> info, UIElement label)
		{
			if (CustomView != null)
			{
				CustomView(info, label);
			}
		}

		/// <summary>
		/// Occurs when label provider is changed.
		/// Notifies axis to update its view.
		/// </summary>
		public event EventHandler Changed;
		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}

		private readonly ResourcePool<UIElement> pool = new ResourcePool<UIElement>();
		internal void ReleaseLabel(UIElement label)
		{
			if (ReleaseCore(label))
			{
				pool.Put(label);
			}
		}

		protected virtual bool ReleaseCore(UIElement label) { return false; }

		protected UIElement GetResourceFromPool()
		{
			return pool.Get();
		}
	}
}
