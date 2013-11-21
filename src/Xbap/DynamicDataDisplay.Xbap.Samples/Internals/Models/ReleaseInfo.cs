using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models
{
	[ContentProperty("Demonstrations")]
	public class ReleaseInfo
	{
		public ReleaseVersion Version { get; set; }

		private readonly ObservableCollection<Demonstration> demonstrations = new ObservableCollection<Demonstration>();
	
		public ObservableCollection<Demonstration> Demonstrations { get { return demonstrations; } }
	}
}
