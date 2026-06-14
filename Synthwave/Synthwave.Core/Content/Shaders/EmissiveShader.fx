float4x4 World;
float4x4 View;
float4x4 Projection;

texture DiffuseMap;
texture EmissiveMap;

float EmissiveStrength = 0;

sampler2D DiffuseSampler = sampler_state
{
    Texture = <DiffuseMap>;
};

sampler2D EmissiveSampler = sampler_state
{
    Texture = <EmissiveMap>;
};

float3 LightDirection = normalize(float3(-0.4f,-1,-0.3));
float AmbientIntensity = 0.35f;

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
    float3 Normal   : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
};

VSOutput VSMain(VSInput input)
{
    VSOutput output;

    float4 worldPos = mul(input.Position, World);

    output.Position =
        mul(
            mul(worldPos, View),
            Projection);

    output.Normal =
        normalize(
            mul(input.Normal,
                (float3x3)World));

    output.TexCoord = input.TexCoord;

    return output;
}

float4 PSMain(VSOutput input) : COLOR0
{
    float3 normal = normalize(input.Normal);

    float NdotL =
        saturate(dot(normal, -LightDirection));

    float lighting =
        AmbientIntensity + NdotL;

    float4 diffuse =
        tex2D(DiffuseSampler,
              input.TexCoord);

    diffuse.rgb *= lighting;

    float3 emissive =
        tex2D(
            EmissiveSampler,
            input.TexCoord).rgb;

    diffuse.rgb +=
        emissive * EmissiveStrength;

    return diffuse;
}

technique Emissive
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}