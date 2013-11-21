using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	[SkipPropertyCheck]
	public class MercatorShader : ShaderEffect
	{

		#region Constructors

		static MercatorShader()
		{
			_pixelShader.UriSource = Global.MakePackUri("MercatorShader.ps");
		}

		public MercatorShader()
		{
			this.PixelShader = _pixelShader;

			// Update each DependencyProperty that's registered with a shader register.  This
			// is needed to ensure the shader gets sent the proper default value.
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(YDiffProperty);
			UpdateShaderValue(YLatDiffProperty);
			UpdateShaderValue(YLatMaxProperty);
			UpdateShaderValue(YMaxProperty);
		}

		#endregion

		#region Dependency Properties

		public Brush Input
		{
			get { return (Brush)GetValue(InputProperty); }
			set { SetValue(InputProperty, value); }
		}

		// Brush-valued properties turn into sampler-property in the shader.
		// This helper sets "ImplicitInput" as the default, meaning the default
		// sampler is whatever the rendering of the element it's being applied to is.
		public static readonly DependencyProperty InputProperty =
			ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(MercatorShader), 0);

		#region YMax property

		public double YMax
		{
			get { return (double)GetValue(YMaxProperty); }
			set { SetValue(YMaxProperty, value); }
		}

		public static readonly DependencyProperty YMaxProperty = DependencyProperty.Register(
		  "YMax",
		  typeof(double),
		  typeof(MercatorShader),
		  new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

		#endregion

		#region YDiff property

		public double YDiff
		{
			get { return (double)GetValue(YDiffProperty); }
			set { SetValue(YDiffProperty, value); }
		}

		public static readonly DependencyProperty YDiffProperty = DependencyProperty.Register(
		  "YDiff",
		  typeof(double),
		  typeof(MercatorShader),
		  new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(1)));

		#endregion

		#region YLatMax property

		public double YLatMax
		{
			get { return (double)GetValue(YLatMaxProperty); }
			set { SetValue(YLatMaxProperty, value); }
		}

		public static readonly DependencyProperty YLatMaxProperty = DependencyProperty.Register(
		  "YLatMax",
		  typeof(double),
		  typeof(MercatorShader),
		  new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(2)));

		#endregion

		#region YLatDiff property

		public double YLatDiff
		{
			get { return (double)GetValue(YLatDiffProperty); }
			set { SetValue(YLatDiffProperty, value); }
		}

		public static readonly DependencyProperty YLatDiffProperty = DependencyProperty.Register(
		  "YLatDiff",
		  typeof(double),
		  typeof(MercatorShader),
		  new FrameworkPropertyMetadata(0.0, PixelShaderConstantCallback(3)));

		#endregion

		#region Scale property

		public double Scale
		{
			get { return (double)GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}

		public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
		  "Scale",
		  typeof(double),
		  typeof(MercatorShader),
		  new FrameworkPropertyMetadata(27.0, PixelShaderConstantCallback(4)));

		#endregion

		#endregion

		#region Member Data

		private static PixelShader _pixelShader = new PixelShader();

		#endregion

	}
}
