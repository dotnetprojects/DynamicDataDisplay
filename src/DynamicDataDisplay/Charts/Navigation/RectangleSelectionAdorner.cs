using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>Helper class to draw semitransparent rectangle over the
    /// selection area</summary>
	public sealed class RectangleSelectionAdorner : Adorner {

		private Rect? border = null;
		public Rect? Border {
			get { return border; }
			set { border = value; }
		}

		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public static readonly DependencyProperty FillProperty =
			DependencyProperty.Register(
			  "Fill",
			  typeof(Brush),
			  typeof(RectangleSelectionAdorner),
			  new FrameworkPropertyMetadata(
				  new SolidColorBrush(Color.FromArgb(60, 100, 100, 100)),
				  FrameworkPropertyMetadataOptions.AffectsRender));

		private Pen pen;
		public Pen Pen {
			get { return pen; }
			set { pen = value; }
		}

		public RectangleSelectionAdorner(UIElement element)
			: base(element) {
			pen = new Pen(Brushes.Black, 1.0);
		}

		protected override void OnRender(DrawingContext dc) {
			if (border.HasValue) {
				dc.DrawRectangle(Fill, pen, border.Value);
			}
		}
	}
}
