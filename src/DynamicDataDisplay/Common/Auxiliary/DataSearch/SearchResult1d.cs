using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.DataSearch
{
	internal struct SearchResult1d
	{
		public static SearchResult1d Empty
		{
			get { return new SearchResult1d { Index = -1 }; }
		}

		public int Index { get; internal set; }

		public bool IsEmpty
		{
			get { return Index == -1; }
		}

		public override string ToString()
		{
			if (IsEmpty)
				return "Empty";

			return String.Format("Index = {0}", Index);
		}
	}
}
