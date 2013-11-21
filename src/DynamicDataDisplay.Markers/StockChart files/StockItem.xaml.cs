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

namespace DynamicDataDisplay.Markers
{
	/// <summary>
	/// Interaction logic for StockItems1.xaml
	/// </summary>
	public partial class StockItem : Control
	{
		static StockItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(StockItem), new FrameworkPropertyMetadata(typeof(StockItem)));
		}

		public StockItem()
		{
			InitializeComponent();
		}

		#region Properties

		public double Open
		{
			get { return (double)GetValue(OpenProperty); }
			set { SetValue(OpenProperty, value); }
		}

		public static readonly DependencyProperty OpenProperty = DependencyProperty.Register(
		  "Open",
		  typeof(double),
		  typeof(StockItem),
		  new FrameworkPropertyMetadata(0.0));

		public double Low
		{
			get { return (double)GetValue(LowProperty); }
			set { SetValue(LowProperty, value); }
		}

		public static readonly DependencyProperty LowProperty = DependencyProperty.Register(
		  "Low",
		  typeof(double),
		  typeof(StockItem),
		  new FrameworkPropertyMetadata(0.0));

		public double High
		{
			get { return (double)GetValue(HighProperty); }
			set { SetValue(HighProperty, value); }
		}

		public static readonly DependencyProperty HighProperty = DependencyProperty.Register(
		  "High",
		  typeof(double),
		  typeof(StockItem),
		  new FrameworkPropertyMetadata(0.0));

		public double Close
		{
			get { return (double)GetValue(CloseProperty); }
			set { SetValue(CloseProperty, value); }
		}

		public static readonly DependencyProperty CloseProperty = DependencyProperty.Register(
		  "Close",
		  typeof(double),
		  typeof(StockItem),
		  new FrameworkPropertyMetadata(0.0));

		//#region Visual styles

		//public Style TopLineStyle
		//{
		//    get { return (Style)GetValue(TopLineStyleProperty); }
		//    set { SetValue(TopLineStyleProperty, value); }
		//}

		//public static readonly DependencyProperty TopLineStyleProperty = DependencyProperty.Register(
		//  "TopLineStyle",
		//  typeof(Style),
		//  typeof(StockItem),
		//  new FrameworkPropertyMetadata(null));

		//public Style CentralRectangleStyle
		//{
		//    get { return (Style)GetValue(CentralRectangleStyleProperty); }
		//    set { SetValue(CentralRectangleStyleProperty, value); }
		//}

		//public static readonly DependencyProperty CentralRectangleStyleProperty = DependencyProperty.Register(
		//  "CentralRectangleStyle",
		//  typeof(Style),
		//  typeof(StockItem),
		//  new FrameworkPropertyMetadata(null));

		//public Style BottomLineStyle
		//{
		//    get { return (Style)GetValue(BottomLineStyleProperty); }
		//    set { SetValue(BottomLineStyleProperty, value); }
		//}

		//public static readonly DependencyProperty BottomLineStyleProperty = DependencyProperty.Register(
		//  "BottomLineStyle",
		//  typeof(Style),
		//  typeof(StockItem),
		//  new FrameworkPropertyMetadata(null));

		//#endregion // end of Visual styles

		#endregion // end of Properties

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var template = Template;
			topRow = (RowDefinition)template.FindName("topRow", this);
			centralRow = (RowDefinition)template.FindName("centralRow", this);
			bottomRow = (RowDefinition)template.FindName("bottomRow", this);
		}

		RowDefinition topRow;
		RowDefinition centralRow;
		RowDefinition bottomRow;

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var max = Math.Max(Open, Close);
			var min = Math.Min(Open, Close);
			var high = High;
			var low = Low;

			if (high - low == 0.0) return;

			topRow.Height = new GridLength((high - max) / (high - low), GridUnitType.Star);
			centralRow.Height = new GridLength((max - min) / (high - low), GridUnitType.Star);
			bottomRow.Height = new GridLength((min - low) / (high - low), GridUnitType.Star);
		}
	}
}
