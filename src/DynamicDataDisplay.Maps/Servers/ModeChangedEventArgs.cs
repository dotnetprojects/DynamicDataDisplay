using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers
{
	public sealed class ModeChangedEventArgs : EventArgs
	{
		private readonly TileSystemMode mode;
		public ModeChangedEventArgs(TileSystemMode mode)
		{
			this.mode = mode;
		}

		public TileSystemMode Mode
		{
			get { return mode; }
		}
	}
}
