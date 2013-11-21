using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering
{
	[ContentProperty("VisualToRender")]
	public class OneThreadRenderingMap : RenderingMap
	{
		public OneThreadRenderingMap()
		{
		}

		public OneThreadRenderingMap(FrameworkElement visualToRender)
			: this()
		{
			VisualToRender = visualToRender;
		}

		public FrameworkElement VisualToRender
		{
			get { return RenderTileServer.VisualToRender; }
			set { RenderTileServer.VisualToRender = value; }
		}
	}
}