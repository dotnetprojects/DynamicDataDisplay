float y1 : register(C0);
float ydiff : register(C1);

float yl1 : register(C2);
float yldiff : register(C3);

//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D implicitInputSampler : register(S0);

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float yr = y1 - ydiff * uv.y;
	float e = exp(yr / 24);
	float yl = 360 * atan(e) / 3.14159265358 - 90;
	uv.y = (yl1 - yl) / yldiff;
	return tex2D(implicitInputSampler, uv);
}
