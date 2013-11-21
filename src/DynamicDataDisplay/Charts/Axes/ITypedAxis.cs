using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Describes axis as having ticks type.
	/// Provides access to some typed properties.
	/// </summary>
	/// <typeparam name="T">Axis tick's type.</typeparam>
	public interface ITypedAxis<T>
	{
		/// <summary>
		/// Gets the ticks provider.
		/// </summary>
		/// <value>The ticks provider.</value>
		ITicksProvider<T> TicksProvider { get; }
		/// <summary>
		/// Gets the label provider.
		/// </summary>
		/// <value>The label provider.</value>
		LabelProviderBase<T> LabelProvider { get; }

		/// <summary>
		/// Gets or sets the convertion of tick from double.
		/// Should not be null.
		/// </summary>
		/// <value>The convert from double.</value>
		Func<double, T> ConvertFromDouble { get; set; }
		/// <summary>
		/// Gets or sets the convertion of tick to double.
		/// Should not be null.
		/// </summary>
		/// <value>The convert to double.</value>
		Func<T, double> ConvertToDouble { get; set; }
	}
}
