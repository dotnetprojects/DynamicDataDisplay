using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public static class LiveToolTipService
	{

		# region Properties

		public static object GetToolTip(DependencyObject obj)
		{
			return (object)obj.GetValue(ToolTipProperty);
		}

		public static void SetToolTip(DependencyObject obj, object value)
		{
			obj.SetValue(ToolTipProperty, value);
		}

		public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached(
		  "ToolTip",
		  typeof(object),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(null, OnToolTipChanged));

		private static LiveToolTip GetLiveToolTip(DependencyObject obj)
		{
			return (LiveToolTip)obj.GetValue(LiveToolTipProperty);
		}

		private static void SetLiveToolTip(DependencyObject obj, LiveToolTip value)
		{
			obj.SetValue(LiveToolTipProperty, value);
		}

		private static readonly DependencyProperty LiveToolTipProperty = DependencyProperty.RegisterAttached(
		  "LiveToolTip",
		  typeof(LiveToolTip),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(null));

		#region Opacity

		public static double GetTooltipOpacity(DependencyObject obj)
		{
			return (double)obj.GetValue(TooltipOpacityProperty);
		}

		public static void SetTooltipOpacity(DependencyObject obj, double value)
		{
			obj.SetValue(TooltipOpacityProperty, value);
		}

		public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.RegisterAttached(
		  "TooltipOpacity",
		  typeof(double),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(1.0, OnTooltipOpacityChanged));

		private static void OnTooltipOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			LiveToolTip liveTooltip = GetLiveToolTip(d);
			if (liveTooltip != null)
			{
				liveTooltip.Opacity = (double)e.NewValue;
			}
		}

		#endregion // end of Opacity

		#region IsPropertyProxy property

		public static bool GetIsPropertyProxy(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsPropertyProxyProperty);
		}

		public static void SetIsPropertyProxy(DependencyObject obj, bool value)
		{
			obj.SetValue(IsPropertyProxyProperty, value);
		}

		public static readonly DependencyProperty IsPropertyProxyProperty = DependencyProperty.RegisterAttached(
		  "IsPropertyProxy",
		  typeof(bool),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(false));

		#endregion // end of IsPropertyProxy property

		#endregion

		private static void OnToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement source = (FrameworkElement)d;

			if (e.NewValue == null)
			{
				source.Loaded -= source_Loaded;
				source.ClearValue(LiveToolTipProperty);
			}

			if (GetIsPropertyProxy(source)) return;

			var content = e.NewValue;

			DataTemplate template = content as DataTemplate;
			if (template != null)
			{
				content = template.LoadContent();
			}

			LiveToolTip tooltip = null;
			if (e.NewValue is LiveToolTip)
			{
				tooltip = e.NewValue as LiveToolTip;
			}
			else
			{
				tooltip = new LiveToolTip { Content = content };
			}

			if (tooltip == null && e.OldValue == null)
			{
				tooltip = new LiveToolTip { Content = content };
			}

			if (tooltip != null)
			{
				SetLiveToolTip(source, tooltip);
				if (!source.IsLoaded)
				{
					source.Loaded += source_Loaded;
				}
				else
				{
					AddTooltip(source);
				}
			}
		}

		private static void AddTooltipForElement(FrameworkElement source, LiveToolTip tooltip)
		{
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(source);

			LiveToolTipAdorner adorner = new LiveToolTipAdorner(source, tooltip);
			layer.Add(adorner);
		}

		private static void source_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement source = (FrameworkElement)sender;

			if (source.IsLoaded)
			{
				AddTooltip(source);
			}
		}

		private static void AddTooltip(FrameworkElement source)
		{
			if (DesignerProperties.GetIsInDesignMode(source)) return;

			LiveToolTip tooltip = GetLiveToolTip(source);

			Window window = Window.GetWindow(source);
			FrameworkElement child = source;
			FrameworkElement parent = null;
			if (window != null)
			{
				while (parent != window)
				{
					parent = (FrameworkElement)VisualTreeHelper.GetParent(child);
					child = parent;
					var nameScope = NameScope.GetNameScope(parent);
					if (nameScope != null)
					{
						string nameScopeName = nameScope.ToString();
						if (nameScopeName != "System.Windows.TemplateNameScope")
						{
							NameScope.SetNameScope(tooltip, nameScope);
							break;
						}
					}
				}
			}

			var binding = BindingOperations.GetBinding(tooltip, LiveToolTip.ContentProperty);
			if (binding != null)
			{
				BindingOperations.ClearBinding(tooltip, LiveToolTip.ContentProperty);
				BindingOperations.SetBinding(tooltip, LiveToolTip.ContentProperty, binding);
			}

			Binding dataContextBinding = new Binding { Path = new PropertyPath("DataContext"), Source = source };
			tooltip.SetBinding(LiveToolTip.DataContextProperty, dataContextBinding);

			tooltip.Owner = source;
			if (GetTooltipOpacity(source) != (double)LiveToolTipService.TooltipOpacityProperty.DefaultMetadata.DefaultValue)
			{
				tooltip.Opacity = LiveToolTipService.GetTooltipOpacity(source);
			}

			AddTooltipForElement(source, tooltip);
		}
	}
}
