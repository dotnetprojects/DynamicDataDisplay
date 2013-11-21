using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models
{
	[TypeConverter(typeof(VersionTypeConverter))]
	public class ReleaseVersion
	{
		public ReleaseVersion(int major, int minor, int revision)
		{
			this.Major = major;
			this.Minor = minor;
			this.Revision = revision;
		}

		public int Major { get; set; }
		public int Minor { get; set; }
		public int Revision { get; set; }

		public override string ToString()
		{
			return String.Format("{0}.{1}.{2}", Major, Minor, Revision);
		}
	}
}
