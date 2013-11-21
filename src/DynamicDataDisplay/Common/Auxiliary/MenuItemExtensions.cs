using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class MenuItemExtensions
	{
		public static MenuItem FindChildByHeader(this MenuItem parent, string header)
		{
			return parent.Items.OfType<MenuItem>().Where(subMenu => subMenu.Header == header).FirstOrDefault();
		}
	}
}
