using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>Base class for all navigation providers</summary>
	public abstract class NavigationBase : ViewportElement2D {
		protected NavigationBase() {
			ManualTranslate = true;
			ManualClip = true;
			Loaded += NavigationBase_Loaded;
		}

		private void NavigationBase_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded(e);
		}

		protected virtual void OnLoaded(RoutedEventArgs e) { }

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Color color = Color.FromArgb(0, 0, 0, 0);
			dc.DrawRectangle(new SolidColorBrush(color), null, state.Output);
		}
	}
}
