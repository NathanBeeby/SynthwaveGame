float Time;
texture SceneTex;
texture BloomTex;

sampler SceneSampler = sampler_state { Texture = <SceneTex>; MinFilter = Linear; MagFilter = Linear; MipFilter = None; };
sampler BloomSampler = sampler_state { Texture = <BloomTex>; MinFilter = Linear; MagFilter = Linear; MipFilter = None; };

struct VS_INPUT
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output;
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float4 scene = tex2D(SceneSampler, input.TexCoord);
    float4 bloom = tex2D(BloomSampler, input.TexCoord);
    return scene + bloom;
}

technique PostProcess
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}