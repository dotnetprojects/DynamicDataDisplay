using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay;
using System.Diagnostics;

namespace DynamicDataDisplay.Markers.ForestDisplay
{
	/// <summary>
	/// Represents an information about tree size and its species.
	/// </summary>
	[DebuggerDisplay("X={X}")]
	public sealed class ForestItem : INotifyPropertyChanged
	{
		private double crownHeight;
		private double crownWidth;
		private string speciesId;
		private double treeHeight;
		private double trunkHeight;
		private double trunkWidth;
		private double x;

		public double CrownWidth
		{
			get { return crownWidth; }
			set
			{
				crownWidth = value;
				RaisePropertyChanged("CrownWidth");
			}
		}

		public double CrownHeight
		{
			get { return crownHeight; }
			set
			{
				crownHeight = value;
				TrunkHeight = treeHeight - crownHeight;
				RaisePropertyChanged("CrownHeight");
			}
		}

		public double TrunkHeight
		{
			get { return trunkHeight; }
			private set
			{
				trunkHeight = value;
				RaisePropertyChanged("TrunkHeight");
			}
		}

		public double TrunkWidth
		{
			get { return trunkWidth; }
			set
			{
				trunkWidth = value;
				RaisePropertyChanged("TrunkWidth");
			}
		}

		public double TreeHeight
		{
			get { return treeHeight; }
			set
			{
				treeHeight = value;
				TrunkHeight = treeHeight - crownHeight;
				RaisePropertyChanged("TreeHeight");
			}
		}

		public string SpeciesID
		{
			get { return speciesId; }
			set
			{
				speciesId = value;
				RaisePropertyChanged("SpeciesID");
			}
		}

		public double X
		{
			get { return x; }
			set
			{
				x = value;
				RaisePropertyChanged("X");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged.Raise(this, propertyName);
		}
	}
}