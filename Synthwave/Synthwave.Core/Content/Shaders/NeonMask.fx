float Time;

texture SceneTexture;

sampler2D SceneSampler = sampler_state
{
    Texture = <SceneTexture>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(VertexShaderOutput input) : COLOR0
{
    float4 col = tex2D(SceneSampler, input.TexCoord);

    // emissive neon mask (bright areas)
    float glow = sin(Time * 5 + input.TexCoord.x * 20) * 0.5 + 0.5;

    return col * glow;
}

technique NeonMask
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}