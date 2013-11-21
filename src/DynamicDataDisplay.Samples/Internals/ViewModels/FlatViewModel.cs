using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.ViewModels
{
	public class FlatViewModel
	{
		public FlatViewModel(List<DemonstrationViewModel> demonstrations)
		{
			this.demonstrations = demonstrations;
		}

		private readonly List<DemonstrationViewModel> demonstrations = new List<DemonstrationViewModel>();
		public List<DemonstrationViewModel> Demonstrations { get { return demonstrations; } }
	}
}
