using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
	/// <summary>Base class for all navigation providers</summary>
	public abstract class NavigationBase : ViewportElement2D
	{
		protected NavigationBase()
		{
			ManualTranslate = true;
			ManualClip = true;
			Loaded += NavigationBase_Loaded;
		}

		private void NavigationBase_Loaded(object sender, RoutedEventArgs e)
		{
			OnLoaded(e);
		}

		protected virtual void OnLoaded(RoutedEventArgs e)
		{
			// this call enables contextMenu to be shown after loading and
			// before any changes to Viewport - without this call 
			// context menu was not shown.
			InvalidateVisual();
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state)
		{
			dc.DrawRectangle(Brushes.Transparent, null, state.Output);
		}
	}
}
