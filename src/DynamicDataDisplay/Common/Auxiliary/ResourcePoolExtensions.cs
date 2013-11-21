using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class ResourcePoolExtensions
	{
		public static T GetOrCreate<T>(this ResourcePool<T> pool) where T : new()
		{
			T instance = pool.Get();
			if (instance == null)
			{
				instance = new T();
			}

			return instance;
		}
	}
}
