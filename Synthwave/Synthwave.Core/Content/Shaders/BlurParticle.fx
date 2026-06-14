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

float2 Direction;

float4 PS(PSInput input) : COLOR
{
    float4 color = 0;

    float offsets[5] = {-2,-1,0,1,2};

    for(int i=0;i<5;i++)
    {
        color += tex2D(TextureSampler, input.UV + Direction * offsets[i] * 0.003);
    }

    return color / 5;
}

technique ParticleBlur
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}