using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation.Peers;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public class PlotterAutomationPeer : FrameworkElementAutomationPeer
	{
		public PlotterAutomationPeer(Plotter owner)
			: base(owner)
		{

		}

		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Custom;
		}

		protected override string GetClassNameCore()
		{
			return "Plotter";
		}
	}
}
