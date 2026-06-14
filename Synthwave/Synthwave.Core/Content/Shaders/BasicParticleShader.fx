struct VSInput
{
    float4 Position : POSITION0;
    float2 UV       : TEXCOORD0;
};

struct PSInput
{
    float4 Position : POSITION0;
    float2 UV       : TEXCOORD0;
};

PSInput VS(VSInput input)
{
    PSInput output;
    output.Position = input.Position;
    output.UV = input.UV;
    return output;
}

sampler TextureSampler;

float4 PS(PSInput input) : COLOR
{
    float4 tex = tex2D(TextureSampler, input.UV);
    float3 glow = tex.rgb;

    return float4(glow, tex.a);
}

technique BasicParticle
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}