
//////////////////////////////////////////////////////////////////
// Shader constants

// some matrices
float4x4 g_ObjectToClip;
float4x4 g_ObjectToWorld;
float4x4 g_ObjectToView;
float4x4 g_ViewToTex;

float4 g_SamplingParams;	// x: sampling interval, y: intensity scale, zw: rec. focal lengths
float4 g_TexCoordOffset;	// tex coord offset to scroll the density mod texture
float3 g_TextureSize;		// size of the EmiAbs texture, needed for improved sampling
float4 g_ClearValue;		// value to clear FP render targets with full screen triangle
float2 g_PixelOffset;		// used to compensate dx9's half pixel offset during rasterisation

Texture3D g_EmiAbsTexture;		// texture with emission (rgb) and absorption (a) properties
Texture2D g_DensityModTexture;	// used to modulate EmiAbsTexture

Texture2D g_EmiAbsAccuBufferTexture;	// render target with ray casting result
Texture2D g_DepthBufferTexture;			// buffer with depths of solid geometry
Texture2D g_EmiAbsDepthBufferTexture;	// buffer with depths of front sides of EmiAbs volumes

float4 denValueRange; //[0-2] - rbg of value [3] - range 
float4 rangeColorVector;

bool EnableSlicing; //determines if we build value-based emi/abs model

const float3x3 RGB2XYZ = 
{0.5141364f, 0.3238786f, 0.16036376f,
0.265068f, 0.67023428f, 0.06409157f,
0.0241188f, 0.1228178f, 0.84442666f}; 


// samplers for the textures
sampler3D g_EmiAbsTextureSampler = 
sampler_state
{
    Texture = <g_EmiAbsTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
	SRGBTexture = TRUE;
};

sampler g_DensityModTextureSampler = 
sampler_state
{
    Texture = <g_DensityModTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
    AddressW = WRAP;

};

sampler2D g_EmiAbsAccuBufferSampler = 
sampler_state
{
    Texture = <g_EmiAbsAccuBufferTexture>;
    MipFilter = LINEAR;//POINT;
    MinFilter = LINEAR;//POINT;
    MagFilter = LINEAR;//POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
	SRGBTexture = TRUE;
};

sampler2D g_DepthBufferSampler = 
sampler_state
{
    Texture = <g_DepthBufferTexture>;
    MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
};

sampler2D g_EmiAbsDepthBufferSampler = 
sampler_state
{
    Texture = <g_EmiAbsDepthBufferTexture>;
    MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
    AddressW = CLAMP;
};


//////////////////////////////////////////////////////////////////
// Shaders

// for simplicity there's only one vertex type for all geometry
struct VSIn_Universal
{
    float3 Position	: POSITION;
};

// same applies to VS output...
struct VSOut_Universal
{
    float4 SVPosition : POSITION;		// homogenous clip position
    float Depth: TEXCOORD0;				// linear depth value
    float4 ClipPosition: TEXCOORD1;		// used to derive screen space texture coordinates
};

// This VS is used for all geometry and passes.
// So for a single pass it will calculate more than needed,
// but I think this makes the code clearer.
VSOut_Universal VS_Universal( VSIn_Universal input )
{
    VSOut_Universal output;
    
    output.SVPosition = mul( float4(input.Position.xyz,1), g_ObjectToClip );
    float3 worldPosition = mul( float4(input.Position.xyz,1), g_ObjectToWorld ).xyz;

   	float4 viewPosition = mul( float4(input.Position.xyz,1), g_ObjectToView );
	output.Depth = viewPosition.z;

	output.ClipPosition = output.SVPosition;

    return output;
}

// This is the main integration method.
// Calculates emission and absorption for a single ray with equidistant samples.
float4 Integrate_Fixed( float3 rayStart_TS, float3 rayIncr_TS, int numSamples, float4 samplingParams )
{
	float3 I = 0;
	float T = 1;
	float4 texCoord = float4( rayStart_TS, 0 );
	for( int i=0; i<numSamples; ++i )
	{
		float4 tex = tex3Dlod( g_EmiAbsTextureSampler, texCoord );
		
		float4 dmod = tex2Dlod( g_DensityModTextureSampler, float4(texCoord.xy+g_TexCoordOffset.xy, 0, 0) );
		float4 f = tex * (1 - g_TexCoordOffset.z*dmod);
		
		if (EnableSlicing)
		{
			float3 dn = saturate(tex.xyz);
			
			//bool checkx = abs(tex.x - denValueRange.x)<denValueRange.w;
			//bool checky = abs(tex.y - denValueRange.y)<denValueRange.w;
			//bool checkz = abs(tex.z - denValueRange.z)<denValueRange.w;
			
			//float3 diff = pow(denValueRange.xyz - tex.xyz, 2.0);
			
			//if (abs(tex.x - denValueRange.x)<denValueRange.w && abs(tex.y - denValueRange.y)<denValueRange.w && abs(tex.z - denValueRange.z)<denValueRange.w && abs(tex.y - denValueRange.y)<denValueRange.w)
			if (abs(denValueRange.x - tex.a)<denValueRange.w)
			{
				T = 10.0;
				I += T*f.rgb;
			}
			else
			{
				T = 0.1f;
				I += T*tex.rgb;
			}
		}
		else
		{
			I += T*tex.rgb;
		}
		
		T *= 1;//pow( saturate(f.a+0.01), samplingParams.x*samplingParams.y );
		
		texCoord.xyz += rayIncr_TS;
	}
	I *= samplingParams.x*samplingParams.y;
	
	return float4( I, T );
}

// PS to accumulate the ray casting result in a separate buffer.
// Reads the depth buffers to get a more robust result.
// Explanation in the accompanying article.
float4 PS_EmiAbsAccu( VSOut_Universal input ) : COLOR0
{
	input.ClipPosition.xy /= input.ClipPosition.w;
	input.ClipPosition.xy += g_PixelOffset;
	float2 screenTexCoord = input.ClipPosition*float2(0.5, -0.5) + float2(0.5,0.5);

	float3 rayDir_VS = float3(
		input.ClipPosition.x * g_SamplingParams.z,
		input.ClipPosition.y * g_SamplingParams.w,
		1
		);
	
	float startDepth = tex2D( g_EmiAbsDepthBufferSampler, screenTexCoord ).r;
	float sceneDepth = tex2D( g_DepthBufferSampler, screenTexCoord ).r;
	float endDepth = min( input.Depth, sceneDepth );
	clip( endDepth - startDepth );
	
	float3 rayStart_VS = rayDir_VS * startDepth;
	float3 rayEnd_VS = rayDir_VS * endDepth;
	float3 rayDiff_VS = rayDir_VS * (endDepth-startDepth);
	float3 rayDiff_TS = mul( rayDiff_VS, (float3x3)g_ViewToTex );

	float3 m = abs( rayDiff_TS ) * g_TextureSize;
	float scale = 1.0 / max( m.x, max(m.y, m.z) );
	float3 rayIncr_TS = rayDiff_TS * g_SamplingParams.x*10 * scale;

	int numSamples = length(rayDiff_TS) / length(rayIncr_TS);
	numSamples = clamp( 0, 1000, numSamples );
	float samplingInterval = 1.0/numSamples * length( rayDiff_VS );
	float4 samplingParams = g_SamplingParams;
	samplingParams.x = samplingInterval;
	
	float3 rayStart_TS = mul( float4(rayStart_VS,1), g_ViewToTex ).xyz;
	rayStart_TS += rayIncr_TS * 0.5;

	//return float4(1,0,0,1);
	return Integrate_Fixed( rayStart_TS, rayIncr_TS, numSamples, samplingParams );
}

// Blends the EmiAbsAccu buffer onto the scene.
float4 PS_EmiAbsCompose( VSOut_Universal input ) : COLOR0
{
	input.ClipPosition.xy /= input.ClipPosition.w;
	input.ClipPosition.xy += g_PixelOffset;
	float2 texCoord = input.ClipPosition*float2(0.5, -0.5) + float2(0.5,0.5);

	float4 ret = tex2D( g_EmiAbsAccuBufferSampler, texCoord );
	
	return ret;
}

// Simply outputs depth of a fragment
float4 PS_Depth( VSOut_Universal input ) : COLOR0
{
	return float4(input.Depth,0,0,0);
}

// To 'shade' the gray solid box
float4 PS_Solid( VSOut_Universal input ) : COLOR0
{
	return float4(0.25,0.25,0.25,1.0);
}


//////////////////////////////////////////////////////////////////
// Techniques

technique EmiAbs_Accu
{
    pass P0
    {          
        VertexShader = compile vs_3_0 VS_Universal();
        PixelShader  = compile ps_3_0 PS_EmiAbsAccu(); 

		ZEnable = FALSE;
		ZWriteEnable = FALSE;
		ZFunc = LESSEQUAL;
		StencilEnable  = FALSE;

		CullMode = CW;

		AlphaBlendEnable = FALSE;
		AlphaTestEnable = FALSE;
		SRGBWriteEnable = TRUE;
	}
}


technique EmiAbs_Compose
{
    pass P0
    {          
        VertexShader = compile vs_3_0 VS_Universal();
        PixelShader  = compile ps_3_0 PS_EmiAbsCompose(); 

		ZEnable = FALSE;
		ZWriteEnable = FALSE;
		ZFunc = LESSEQUAL;
		StencilEnable  = FALSE;

		CullMode = CW;

		AlphaBlendEnable = TRUE;
		SrcBlend = ONE;
		DestBlend = SRCALPHA;
		BlendOp = ADD;
		SrcBlendAlpha = ZERO;
		DestBlendAlpha = ZERO;
		BlendOpAlpha = ADD;
		AlphaTestEnable = FALSE;
		SRGBWriteEnable = TRUE;
	}
}


technique Depth
{
    pass P0
    {          
        VertexShader = compile vs_3_0 VS_Universal();
        PixelShader  = compile ps_3_0 PS_Depth(); 

		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		ZFunc = LESSEQUAL;
		StencilEnable  = FALSE;

		CullMode = CCW;

		AlphaBlendEnable = FALSE;
		SRGBWriteEnable = FALSE;
	}
}


technique Solid
{
    pass P0
    {          
        VertexShader = compile vs_3_0 VS_Universal();
        PixelShader  = compile ps_3_0 PS_Solid(); 

		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		ZFunc = LESSEQUAL;
		StencilEnable  = FALSE;

		CullMode = CCW;

		AlphaBlendEnable = FALSE;
		SRGBWriteEnable = TRUE;
	}
}




//////////////////////////////////////////////////////////////////
// Shaders and technique to clear FP render targets

float4 VS_Clear( float3 pos : POSITION) : POSITION
{
	pos.xy -= g_PixelOffset.xy;
	return float4(pos,1);
}

float4 PS_Clear(): COLOR0
{
	return g_ClearValue;
}

technique ClearRenderTarget
{
    pass P0
    {          
        VertexShader = compile vs_3_0 VS_Clear();
        PixelShader  = compile ps_3_0 PS_Clear(); 

		ZEnable = FALSE;
		ZWriteEnable = FALSE;
		ZFunc = LESSEQUAL;
		StencilEnable  = FALSE;

		CullMode = CCW;

		AlphaBlendEnable = FALSE;
		AlphaTestEnable = FALSE;
		SRGBWriteEnable = FALSE;
	}
}

