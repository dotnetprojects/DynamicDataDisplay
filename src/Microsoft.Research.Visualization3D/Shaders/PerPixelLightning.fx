shared float4x4 world;
shared float4x4 view;
shared float4x4 projection;
shared float3 cameraPosition;

//light properties
shared float3 lightPosition;
shared float4 ambientLightColor;
shared float4 diffuseLightColor;
shared float4 specularLightColor;

//material properties
shared float specularPower;
shared float specularIntensity;

shared float4 mbColor;

struct VertexShaderOutputPerPixelDiffuse
{
     float4 Position : POSITION;
     float3 WorldNormal : TEXCOORD0;
     float3 WorldPosition : TEXCOORD1;
     float3 Color : COLOR;
};

struct PixelShaderInputPerPixelDiffuse
{
     float3 WorldNormal : TEXCOORD0;
     float3 WorldPosition : TEXCOORD1;
     float4 Color : COLOR;
};

struct PixelShaderMetaballInputPerPixelDiffuse
{
     float3 WorldNormal : TEXCOORD0;
     float3 WorldPosition : TEXCOORD1;
};

struct VertexShaderMetaballOutputPerPixelDiffuse
{
     float4 Position : POSITION;
     float3 WorldNormal : TEXCOORD0;
     float3 WorldPosition : TEXCOORD1;
};

VertexShaderOutputPerPixelDiffuse PerPixelDiffuseVS(
     float3 position : POSITION,
     float3 normal : NORMAL,
     float3 color : COLOR )
{
     VertexShaderOutputPerPixelDiffuse output;

     //generate the world-view-projection matrix
     float4x4 wvp = mul(mul(world, view), projection);
     
     //transform the input position to the output
     output.Position = mul(float4(position, 1.0), wvp);

     output.WorldNormal =  mul(normal, world);
     float4 worldPosition =  mul(float4(position, 1.0), world);
     output.WorldPosition = worldPosition / worldPosition.w;
     
     output.Color = color;

     //return the output structure
     return output;
}

VertexShaderMetaballOutputPerPixelDiffuse PerPixelDiffuseMetaballVS(
     float3 position : POSITION,
     float3 normal : NORMAL
     )
{
     VertexShaderMetaballOutputPerPixelDiffuse output;

     //generate the world-view-projection matrix
     float4x4 wvp = mul(mul(world, view), projection);
     
     //transform the input position to the output
     output.Position = mul(float4(position, 1.0), wvp);

     output.WorldNormal =  mul(normal, world);
     float4 worldPosition =  mul(float4(position, 1.0), world);
     output.WorldPosition = worldPosition / worldPosition.w;
     
     //return the output structure
     return output;
}

float4 DiffuseAndPhongPS(PixelShaderInputPerPixelDiffuse input) : COLOR
{
     //calculate per-pixel diffuse
     float3 directionToLight = normalize(lightPosition - input.WorldPosition);
     float diffuseIntensity = saturate( dot(directionToLight, input.WorldNormal));
     float4 diffuse = diffuseLightColor * diffuseIntensity;

     //calculate Phong components per-pixel
     float3 reflectionVector = normalize(reflect(-directionToLight, input.WorldNormal));
     float3 directionToCamera = normalize(cameraPosition - input.WorldPosition);
     
     //calculate specular component
     float4 specular = specularLightColor * specularIntensity * 
                       pow(saturate(dot(reflectionVector, directionToCamera)), 
                           specularPower);
      
     //all color components are summed in the pixel shader
     float4 color = input.Color * (specular  + diffuse + ambientLightColor);
     color.a = 1.0;
     return color;
}

float4 DiffuseAndPhongMetaballPS(PixelShaderMetaballInputPerPixelDiffuse input) : COLOR
{
     //calculate per-pixel diffuse
     float3 directionToLight = normalize(lightPosition - input.WorldPosition);
     float diffuseIntensity = saturate( dot(directionToLight, input.WorldNormal));
     float4 diffuse = diffuseLightColor * diffuseIntensity;

     //calculate Phong components per-pixel
     float3 reflectionVector = normalize(reflect(-directionToLight, input.WorldNormal));
     float3 directionToCamera = normalize(cameraPosition - input.WorldPosition);
     
     //calculate specular component
     float4 specular = specularLightColor * specularIntensity * 
                       pow(saturate(dot(reflectionVector, directionToCamera)), 
                           specularPower);
      
     //all color components are summed in the pixel shader
     float4 color = mbColor * (specular  + diffuse + ambientLightColor);
     color.a = 1.0;
     return color;
}

technique PerPixelDiffuseAndPhong
{
     
    pass P0
    {
          //DestBlend = ONE;
		  //SrcBlend = ONE;
		  //ZEnable = TRUE;
		  //ZWriteEnable = FALSE;
		  //CullMode = NONE;
	          
          //set the VertexShader state to the basic
          //vertex shader that will set up inputs for
          //the pixel shader
          VertexShader = compile vs_2_0 PerPixelDiffuseVS();
          
          //set the PixelShader state to the complete Phong Shading
          //implementation of the pixel shader       
          PixelShader = compile ps_2_0 DiffuseAndPhongPS();
    }
}

technique PerPixelDiffuseAndPhongMetaball
{
     
    pass P0
    {
          //DestBlend = ONE;
		  //SrcBlend = ONE;
		  //ZEnable = TRUE;
		  //ZWriteEnable = FALSE;
		  //CullMode = NONE;
	          
          //set the VertexShader state to the basic
          //vertex shader that will set up inputs for
          //the pixel shader
          VertexShader = compile vs_2_0 PerPixelDiffuseMetaballVS();
          
          //set the PixelShader state to the complete Phong Shading
          //implementation of the pixel shader       
          PixelShader = compile ps_2_0 DiffuseAndPhongMetaballPS();
    }
}
