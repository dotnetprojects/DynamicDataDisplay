using System.Windows.Input;

namespace Microsoft.Research.DynamicDataDisplay
{
	// TODO: probably optimize memory usage by replacing implicit creation of 
	// all commands on first usage of this class - 
	// create each command on first access directly to it.

	/// <summary>This class holds keyboard shortcuts to common viewport navigation commands</summary>
	public static class ChartCommands
	{

		#region Auxiliary code for creation of commands

		private static RoutedUICommand CreateCommand(string name)
		{
			return new RoutedUICommand(name, name, typeof(ChartCommands));
		}

		private static RoutedUICommand CreateCommand(string name, params Key[] keys)
		{
			InputGestureCollection gestures = new InputGestureCollection();
			foreach (var key in keys)
			{
				gestures.Add(new KeyGesture(key));
			}
			return new RoutedUICommand(name, name, typeof(ChartCommands), gestures);
		}

		private static RoutedUICommand CreateCommand(string name, params InputGesture[] gestures)
		{
			return new RoutedUICommand(name, name, typeof(ChartCommands), new InputGestureCollection(gestures));
		}

		#endregion

		private static readonly RoutedUICommand zoomWithParam = CreateCommand("ZoomWithParam");
		public static RoutedUICommand ZoomWithParameter
		{
			get { return zoomWithParam; }
		}

		private static readonly RoutedUICommand zoomIn = CreateCommand("ZoomIn", Key.OemPlus);
		public static RoutedUICommand ZoomIn
		{
			get { return zoomIn; }
		}

		private static readonly RoutedUICommand zoomOut = CreateCommand("ZoomOut", Key.OemMinus);
		public static RoutedUICommand ZoomOut
		{
			get { return zoomOut; }
		}

		private static readonly RoutedUICommand fitToView = CreateCommand("FitToView", Key.Home);
		public static RoutedUICommand FitToView
		{
			get { return ChartCommands.fitToView; }
		}

		private static readonly RoutedUICommand scrollLeft = CreateCommand("ScrollLeft", Key.Left);
		public static RoutedUICommand ScrollLeft
		{
			get { return ChartCommands.scrollLeft; }
		}

		private static readonly RoutedUICommand scrollRight = CreateCommand("ScrollRight", Key.Right);
		public static RoutedUICommand ScrollRight
		{
			get { return ChartCommands.scrollRight; }
		}

		private static readonly RoutedUICommand scrollUp = CreateCommand("ScrollUp", Key.Up);
		public static RoutedUICommand ScrollUp
		{
			get { return ChartCommands.scrollUp; }
		}

		private static readonly RoutedUICommand scrollDown = CreateCommand("ScrollDown", Key.Down);
		public static RoutedUICommand ScrollDown
		{
			get { return ChartCommands.scrollDown; }
		}

		private static readonly RoutedUICommand saveScreenshot = CreateCommand("SaveScreenshot", new KeyGesture(Key.S, ModifierKeys.Control));
		public static RoutedUICommand SaveScreenshot
		{
			get { return ChartCommands.saveScreenshot; }
		}

		private static readonly RoutedUICommand copyScreenshot = CreateCommand("CopyScreenshot", Key.F11);
		public static RoutedUICommand CopyScreenshot
		{
			get { return ChartCommands.copyScreenshot; }
		}

		private static readonly RoutedUICommand showHelp = CreateCommand("ShowHelp", Key.F1);
		public static RoutedUICommand ShowHelp
		{
			get { return ChartCommands.showHelp; }
		}
	}
}
