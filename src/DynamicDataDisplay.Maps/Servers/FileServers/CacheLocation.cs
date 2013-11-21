using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps
{
	/// <summary>
	/// Represents a enumeration of tile cache locations - types of base folders used to store tiles.
	/// </summary>
	public enum CacheLocation
	{
		/// <summary>
		/// Tiles are being searched and saved to application folder.
		/// </summary>
		ApplicationFolder,
		/// <summary>
		/// Tiles are being searched and saved to roaming user profile application data folder.
		/// </summary>
		ApplicationDataFolder,
		/// <summary>
		/// Tiles are being searched and saved to temporary internet files (cache of Intenet Explorer).
		/// </summary>
		TemporaryInternetFiles,
        /// <summary>
        /// Tiles are being searched and saved to user profile's Temp folder.
        /// </summary>
		TempFiles,
        /// <summary>
        /// Tiles are being searched on user-specified path.
        /// </summary>
        CustomPath
	}
}
