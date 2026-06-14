float4x4 World;
float4x4 View;
float4x4 Projection;

texture DiffuseMap;

sampler2D DiffuseSampler = sampler_state
{
    Texture = <DiffuseMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};

float3 LightDirection = normalize(float3(-0.4f, -1.0f, -0.3f));
float3 LightColor = float3(1,1,1);
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

    diffuse.rgb *= LightColor * lighting;

    return diffuse;
}

technique Standard
{
    pass P0
    {
        VertexShader = compile vs_3_0 VSMain();
        PixelShader = compile ps_3_0 PSMain();
    }
}