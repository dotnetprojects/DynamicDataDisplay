using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Markup;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>Plotter is a base control for displaying various graphs. It provides
	/// mainCanvas to draw graph itself and side space for axes, annotations, etc</summary>
	[ContentProperty("Children")]
	public partial class Plotter : ContentControl
	{
		static Plotter() {
			//DefaultStyleKeyProperty.OverrideMetadata(typeof(Plotter), new FrameworkPropertyMetadata(typeof(Plotter)));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plotter"/> class.
		/// </summary>
		public Plotter()
		{
			children.ChildAdded += OnChildAdded;
			children.ChildRemoving += OnChildRemoving;

			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			//headerPanel = GetTemplateChild("headerPanel") as StackPanel;
		}

		#region Children and add/removed events handling

		private readonly ObservableChartsCollection children = new ObservableChartsCollection();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ObservableChartsCollection Children
		{
			get { return children; }
		}

		private void OnChildRemoving(object sender, PlotterCollectionChangedEventArgs e)
		{
			OnChildRemoving(e.Child);
		}

		protected virtual void OnChildRemoving(IPlotterElement uiElement)
		{
			IPlotterElement child = uiElement as IPlotterElement;
			if (child != null)
			{
				child.OnPlotterDetaching(this);
			}
		}

		private void OnChildAdded(object sender, PlotterCollectionChangedEventArgs e)
		{
			OnChildAdded(e.Child);
		}

		protected virtual void OnChildAdded(IPlotterElement uiElement)
		{
			IPlotterElement child = uiElement as IPlotterElement;
			if (child != null)
			{
				child.OnPlotterAttached(this);
			}
		}

		#endregion

		#region Layout zones

		public StackPanel HeaderPanel
		{
			get { return headerPanel; }
		}

		public StackPanel FooterPanel
		{
			get { return footerPanel; }
		}

		public StackPanel LeftPanel
		{
			get { return leftPanel; }
		}

		public StackPanel BottomPanel
		{
			get { return bottomPanel; }
		}

		public Canvas MainCanvas
		{
			get { return mainCanvas; }
		}

		public Grid CentralGrid
		{
			get { return centralGrid; }
		}

		#endregion

		#region Screenshots & copy to clipboard

		public BitmapSource CreateScreenshot()
		{
			Window window = Window.GetWindow(this);

			Rect renderBounds = new Rect(RenderSize);

			Point p1 = TranslatePoint(renderBounds.Location, window);
			Point p2 = TranslatePoint(renderBounds.BottomRight, window);

			Int32Rect rect = new Rect(p1, p2).ToInt32Rect();
			rect.Y = 0;

			return ScreenshotHelper.CreateScreenshot(this, rect);
		}


		/// <summary>Saves the screenshot.</summary>
		/// <param name="filePath">The file path.</param>
		public void SaveScreenshot(string filePath)
		{
			ScreenshotHelper.SaveBitmap(CreateScreenshot(), filePath);
		}

		/// <summary>Copies the screenshot to clipboard.</summary>
		public void CopyScreenshotToClipboard()
		{
			Clipboard.Clear();
			Clipboard.SetImage(CreateScreenshot());
		}

		#endregion

		#region IsDefaultElement attached property

		protected void SetAllChildrenAsDefault()
		{
			foreach (var child in Children.OfType<DependencyObject>())
			{
				child.SetValue(IsDefaultElementProperty, true);
			}
		}

		/// <summary>Gets a value whether specified graphics object is default to this plotter or not</summary>
		/// <param name="obj">Graphics object to check</param>
		/// <returns>True if it is default or false otherwise</returns>
		public static bool GetIsDefaultElement(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsDefaultElementProperty);
		}

		public static void SetIsDefaultElement(DependencyObject obj, bool value)
		{
			obj.SetValue(IsDefaultElementProperty, value);
		}

		public static readonly DependencyProperty IsDefaultElementProperty = DependencyProperty.RegisterAttached(
			"IsDefaultElement",
			typeof(bool),
			typeof(Plotter),
			new UIPropertyMetadata(false));

		/// <summary>Removes all user graphs from given UIElementCollection, 
		/// leaving only default graphs</summary>
		protected static void RemoveUserElements(ObservableChartsCollection elements)
		{
			int index = 0;

			while (index < elements.Count)
			{
				DependencyObject d = elements[index] as DependencyObject;
				if (d != null && !GetIsDefaultElement(d))
				{
					elements.RemoveAt(index);
				}
				else
				{
					index++;
				}
			}
		}

		public void RemoveUserElements()
		{
			RemoveUserElements(Children);
		}

		#endregion
	}
}
