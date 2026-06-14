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

sampler SceneSampler;
sampler BloomSampler;

float BloomIntensity;

float4 PS(PSInput input) : COLOR
{
    float4 scene = tex2D(SceneSampler, input.UV);
    float4 bloom = tex2D(BloomSampler, input.UV);

    return scene + bloom * BloomIntensity;
}

technique CombineBloom
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}