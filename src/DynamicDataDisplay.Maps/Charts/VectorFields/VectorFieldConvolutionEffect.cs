using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class VectorFieldConvolutionEffect : ShaderEffect
	{
		#region Constructors

		static VectorFieldConvolutionEffect()
		{
			_pixelShader.UriSource = Global.MakePackUri("Charts/VectorFields/VectorFieldConvolutionEffect.ps");
		}

		public VectorFieldConvolutionEffect()
		{
			this.PixelShader = _pixelShader;

			// Update each DependencyProperty that's registered with a shader register.  This
			// is needed to ensure the shader gets sent the proper default value.
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(ShiftProperty);
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
			ShaderEffect.RegisterPixelShaderSamplerProperty(
			"Input", typeof(VectorFieldConvolutionEffect), 0);

		public Brush Shift
		{
			get { return (Brush)GetValue(ShiftProperty); }
			set { SetValue(ShiftProperty, value); }
		}

		public static readonly DependencyProperty ShiftProperty = 
			ShaderEffect.RegisterPixelShaderSamplerProperty(
			"Shift",
			typeof(VectorFieldConvolutionEffect), 
			2);


		#endregion

		#region Member Data

		private static PixelShader _pixelShader = new PixelShader();

		#endregion
	}
}
