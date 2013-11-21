using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models
{
	[ContentProperty("Releases")]
	public class SamplesCollection
	{
		private readonly ObservableCollection<ReleaseInfo> releases = new ObservableCollection<ReleaseInfo>();
		public ObservableCollection<ReleaseInfo> Releases { get { return releases; } }
	}
}
