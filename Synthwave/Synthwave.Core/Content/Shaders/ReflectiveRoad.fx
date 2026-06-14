float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CameraPosition;

float ReflectionStrength;
float Roughness;

texture DiffuseTexture;
texture EnvTexture;

sampler2D DiffuseSampler = sampler_state
{
    Texture = <DiffuseTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
};

sampler2D EnvSampler = sampler_state
{
    Texture = <EnvTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
};

struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float3 WorldPos : TEXCOORD0;
    float3 Normal   : TEXCOORD1;
    float2 TexCoord : TEXCOORD2;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;

    float4 wp = mul(input.Position, World);
    o.WorldPos = wp.xyz;

    o.Position = mul(wp, View);
    o.Position = mul(o.Position, Projection);

    o.Normal = mul(input.Normal, (float3x3)World);
    o.TexCoord = input.TexCoord;

    return o;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float3 viewDir = normalize(CameraPosition - input.WorldPos);
    float3 normal = normalize(input.Normal);

    float3 refl = reflect(-viewDir, normal);

    float2 envUV = refl.xz * 0.5 + 0.5;

    float4 albedo = tex2D(DiffuseSampler, input.TexCoord);
    float4 env = tex2D(EnvSampler, envUV);

    float fresnel = pow(1.0 - saturate(dot(viewDir, normal)), 5.0);

    env.rgb = lerp(env.rgb, albedo.rgb, Roughness);

    float r = ReflectionStrength * fresnel;

    float3 col = lerp(albedo.rgb, env.rgb, r);

    return float4(col, 1);
}

technique Reflect
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}