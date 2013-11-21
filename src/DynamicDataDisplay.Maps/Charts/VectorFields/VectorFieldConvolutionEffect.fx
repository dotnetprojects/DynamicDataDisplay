//--------------------------------------------------------------------------------------
// 
// WPF ShaderEffect HLSL -- VectorFieldConvolution
//
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D implicitInputSampler : register(S0);
sampler2D shiftSampler : register(S2);


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{
   float4 shift = tex2D(shiftSampler, uv);
   float dx = shift.a;
   float dy = shift.g;
   
   float coord = uv;

   float4 color = tex2D(implicitInputSampler, coord);
   return color;
}


