using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Windows;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	[DebuggerDisplay("Tile = {Tile}, Screen = {ScreenBounds}, Visible = {VisibleBounds}")]
	public sealed class VisibleTileInfo
	{
		public TileIndex Tile { get; set; }
		public Rect ScreenBounds { get; set; }
		public DataRect VisibleBounds { get; set; }
	}
}
