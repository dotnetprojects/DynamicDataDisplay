// Camera parameters.
float4x4 World;
float4x4 View;
float4x4 Projection;
float ViewportHeight;

// The current time, in seconds.
float CurrentTime;

// Parameters describing how the particles animate.
float Duration;
float DurationRandomness;
float3 Gravity;
float EndVelocity;


// These float2 parameters describe the min and max of a range.
// The actual value is chosen differently for each particle,
// interpolating between x and y by some random amount.
float2 RotateSpeed;
float2 StartSize;
float2 EndSize;


// Particle texture and sampler.
Texture2D Texture;

sampler Sampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// Vertex shader input structure describes the start position and
// velocity of the particle, and the time at which it was created,
// along with some random values that affect its size and rotation.
struct VertexShaderInput
{
    float3 Position : POSITION;
    float3 Velocity : NORMAL;
    float3 Random : COLOR;
    float Time : TEXCOORD0;
};


// Vertex shader output structure specifies the position, size, and
// color of the particle, plus a 2x2 rotation matrix (packed into
// a float4 value because we don't have enough color interpolators
// to send this directly as a float2x2).
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float Size : PSIZE0;
    float4 Color : COLOR0;
    float4 Rotation : COLOR1;
};


// Vertex shader helper for computing the position of a particle.
float4 ComputeParticlePosition(float3 position, float3 velocity,
                               float age, float normalizedAge)
{
    
    //float startVelocity = length(velocity);
    //float endVelocity = startVelocity * EndVelocity;
    //float velocityIntegral = startVelocity * normalizedAge + (endVelocity - startVelocity) * normalizedAge * normalizedAge / 2;
    //position += normalize(velocity) * velocityIntegral * Duration;
    //position += Gravity * age * normalizedAge;
    return mul(mul(float4(position, 1), View), Projection);
    
}


// Vertex shader helper for computing the size of a particle.
float ComputeParticleSize(float4 projectedPosition,
                          float randomValue, float normalizedAge)
{
    
    float startSize = lerp(StartSize.x, StartSize.y, randomValue);
    float endSize = lerp(EndSize.x, EndSize.y, randomValue);
    float size = lerp(startSize, endSize, normalizedAge);
    return size * Projection._m11 / projectedPosition.w * ViewportHeight / 2;
    
}

// Vertex shader helper for computing the rotation of a particle.
float4 ComputeParticleRotation(float randomValue, float age)
{    
    
    float rotateSpeed = lerp(RotateSpeed.x, RotateSpeed.y, randomValue);
    float rotation = rotateSpeed * age;
    float c = cos(rotation);
    float s = sin(rotation);
    float4 rotationMatrix = float4(c, -s, s, c);
    rotationMatrix *= 0.5;
    rotationMatrix += 0.5;
    return rotationMatrix;
    
}


// Custom vertex shader animates particles entirely on the GPU.
VertexShaderOutput VShader(VertexShaderInput input)
{
    
    VertexShaderOutput output;
    float age = CurrentTime - input.Time;
    age *= 1 + DurationRandomness;
    float normalizedAge = saturate(age / Duration);
    output.Position = ComputeParticlePosition(input.Position, input.Velocity, age, normalizedAge);
    output.Size = ComputeParticleSize(output.Position, input.Random.y, normalizedAge);
    output.Color = float4(input.Random.xyz, 1);
    output.Rotation = ComputeParticleRotation(input.Random.x, age);
    
    return output;
}


// Pixel shader input structure for particles that can rotate.
struct RotatingPixelShaderInput
{
    float4 Color : COLOR0;
    float4 Rotation : COLOR1;
    float2 TextureCoordinate : TEXCOORD0;
};


// Pixel shader for drawing particles that can rotate. It is not actually
// possible to rotate a point sprite, so instead we rotate our texture
// coordinates. Leaving the sprite the regular way up but rotating the
// texture has the exact same effect as if we were able to rotate the
// point sprite itself.
float4 RotatingPixelShader(RotatingPixelShaderInput input) : COLOR0
{
    
    float2 textureCoordinate = input.TextureCoordinate;
    textureCoordinate -= 0.5;
    float4 rotation = input.Rotation * 2 - 1;
    textureCoordinate = mul(textureCoordinate, float2x2(rotation));
    textureCoordinate *= sqrt(2);
    textureCoordinate += 0.5;
    
    float4 result = tex2D(Sampler, textureCoordinate) * input.Color * 4;
    result.a /= 4;
    
    return result;
    
}

// Effect technique for drawing particles that can rotate. Requires shader 2.0.
technique RotatingParticles
{
    pass P0
    {
        VertexShader = compile vs_3_0 VShader();
        PixelShader = compile ps_3_0 RotatingPixelShader();
    }
}
