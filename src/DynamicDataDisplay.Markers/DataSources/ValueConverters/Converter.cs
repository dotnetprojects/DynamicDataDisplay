using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.Markers.DataSources.ValueConverters
{
	public static class Converter
	{
		public static GenericLambdaConverter<T, object> Create<T>(Func<T, object> lambda)
		{
			return new GenericLambdaConverter<T, object>(lambda);
		}
	}
}
