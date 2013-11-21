float y1 : register(C0);
float ydiff : register(C1);

float yl1 : register(C2);
float yldiff : register(C3);

float scale : register(C4);

sampler2D inputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	if(ydiff > 1) {

		float yr = y1 - ydiff * uv.y;
		//                               3.14159265359 / 360 = 0.008726646259971648 in float
		float yl = scale * log(tan(0.008726646259971648/*0.00872664625997222222222222222222*/ * (yr + 90)));
		uv.y = (yl1 - yl) * yldiff;
		
		//==========================
		// For debug purpose - colors regions with wrong texture coordinates to blue or red.
		//if(uv.y < 0) {
		//	return float4(1,0,0,1);
		//}
		//if(uv.y > 1) {
		//	return float4(0,0,1,1);
		//}
		//==========================
	}
	
	return tex2D(inputSampler, uv);
	
	//==========================
	// Makes ocean color in VE Road transparent.
	 
	// Second color is a blue color of ocean
	//float len = length(color.rgb - float3(0.70196, 0.7764, 0.8313));
	//const float maxLen = 0.09;
	//if(len < maxLen) {
	//	float alpha = len / maxLen;
	//	alpha *= alpha;
	//	return lerp(float4(0,0,0,0), color, alpha);
	//}
	//==========================
	//else {
	//	return color;
	//}
}