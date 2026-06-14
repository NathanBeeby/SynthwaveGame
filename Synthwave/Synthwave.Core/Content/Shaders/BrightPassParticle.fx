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

float Threshold;

float4 PS(PSInput input) : COLOR
{
    float4 color = tex2D(TextureSampler, input.UV);

    float brightness = dot(color.rgb, float3(0.2126, 0.7152, 0.0722));

    if (brightness < Threshold)
        return float4(0,0,0,1);

    return color;
}

technique BrightPass
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}