using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	public interface INonUniformDataSource2D<T> : IDataSource2D<T> where T : struct
	{
		double[] XCoordinates { get; }
		double[] YCoordinates { get; }
	}
}
