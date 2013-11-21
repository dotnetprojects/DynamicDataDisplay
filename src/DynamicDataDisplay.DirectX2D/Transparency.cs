using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D
{
	public class Transparency : ShaderEffect
	{
		#region Constructors

		static Transparency()
		{
			_pixelShader.UriSource = Global.MakePackUri("Transparency.ps");
		}

		public Transparency()
		{
			this.PixelShader = _pixelShader;

			// Update each DependencyProperty that's registered with a shader register.  This
			// is needed to ensure the shader gets sent the proper default value.
			UpdateShaderValue(InputProperty);
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
			ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Transparency), 0);

		#endregion

		#region Member Data

		private static PixelShader _pixelShader = new PixelShader();

		#endregion

	}
}
