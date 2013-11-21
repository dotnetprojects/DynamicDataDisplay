using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
    /// <summary>
    /// Represents a plotter with now extra place around it, e.g. without Left, Right, Top, Bottom panels and without
    /// Footer panel and Header panel. Used in tiled rendering.
    /// </summary>
	public partial class CapturingPlotter : Plotter2D
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CapturingPlotter"/> class.
        /// </summary>
		public CapturingPlotter()
		{
			InitializeComponent();
		}
	}
}
