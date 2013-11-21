using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	/// <summary>
	/// Represents a VirtualEarth Imagery Service Hybrid tile server.
	/// </summary>
	public sealed class VEImageryHybridServer : VEImageryServerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VEImageryHybridServer"/> class.
		/// </summary>
		public VEImageryHybridServer()
		{
			MapStyle = VE.VESvc.MapStyle.AerialWithLabels;
			FileExtension = ".jpg";
			ServerName += "Hybrid";
		}
	}
}
