using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	/// <summary>
	/// Represents a VirtualEarth Imagery Service Aerial tile server.
	/// </summary>
	public sealed class VEImageryAerialServer : VEImageryServerBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VEImageryAerialServer"/> class.
		/// </summary>
		public VEImageryAerialServer()
		{
			MapStyle = VE.VESvc.MapStyle.Aerial;
			FileExtension = ".jpg";
			ServerName += "Aerial";
		}
	}
}
