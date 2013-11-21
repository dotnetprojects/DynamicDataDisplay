sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
   float4 color = tex2D(implicitInputSampler, uv);
   if(color.a == 1 && color.r == 1 && color.b == 1 && color.g == 1)
	color = float4(0, 0, 0, 0);
   return color;
}


