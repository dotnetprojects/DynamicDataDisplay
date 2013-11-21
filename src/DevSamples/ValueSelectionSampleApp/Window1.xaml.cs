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
using System.Diagnostics;

namespace ValueSelectionSampleApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			//AddHandler(FocusManager.GotFocusEvent, (RoutedEventHandler)((s, e) =>
			//{
			//    Debug.WriteLine("Got Focus: " + e.Source);
			//    UIElement element = s as UIElement;
			//    if (element != null)
			//    {
			//        var next = element.PredictFocus(FocusNavigationDirection.Next);
			//    }
			//}));
			//AddHandler(FocusManager.LostFocusEvent, (RoutedEventHandler)((s, e) =>
			//{
			//    Debug.WriteLine("Lost Focus: " + e.Source);
			//}));

			//AddHandler(Keyboard.LostKeyboardFocusEvent, (RoutedEventHandler)Logger);
			//AddHandler(Keyboard.GotKeyboardFocusEvent, (RoutedEventHandler)Logger);
			//AddHandler(Keyboard.PreviewGotKeyboardFocusEvent, (RoutedEventHandler)Logger);
			//AddHandler(Keyboard.PreviewLostKeyboardFocusEvent, (RoutedEventHandler)Logger);
		}

		public void Logger(object sender, RoutedEventArgs e)
		{
			string message = e.RoutedEvent + "Source: " + e.Source;

			KeyboardFocusChangedEventArgs focusEA = e as KeyboardFocusChangedEventArgs;
			message += " NewFocus: " + focusEA.NewFocus;

			Debug.WriteLine(message);
		}
	}
}
