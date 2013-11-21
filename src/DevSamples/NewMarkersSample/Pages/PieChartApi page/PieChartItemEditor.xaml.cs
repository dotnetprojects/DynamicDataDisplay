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
using System.Globalization;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for PieChartItemEditor.xaml
	/// </summary>
	public partial class PieChartItemEditor : UserControl, INotifyPropertyChanged
	{
		public PieChartItemEditor()
		{
			InitializeComponent();
		}

		public void Reset()
		{
			captionTb.Text = "";
			valueTb.Text = "";
			colorSelector.SelectedValue = 0.5;
		}

		public PieChartEditorResult GetValue()
		{
			double value;
			bool hasResult = HasNormalResult(out value);

			if (hasResult)
			{
				PieChartEditorResult result = new PieChartEditorResult { Caption = captionTb.Text, Fill = colorSelector.SelectedBrush, Value = value };
				return result;
			}
			else
			{
				return null;
			}
		}

		private bool HasNormalResult(out double value)
		{
			value = 0.0;
			var hasResult = Double.TryParse(valueTb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
			hasResult &= !String.IsNullOrEmpty(captionTb.Text);
			return hasResult;
		}

		private bool hasResult = false;
		public bool HasResult
		{
			get
			{
				return hasResult;
			}
			private set
			{
				hasResult = value;
				PropertyChanged.Raise(this, "HasResult");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			double v;
			HasResult = HasNormalResult(out v);
		}
	}
}
