// PostProcessingShader.fx
// Composites the scene with an optional bloom layer.
// BloomTex is optional — if not set it contributes nothing (black = additive zero).

float Time;   // used for a subtle vignette pulse so the compiler keeps it

texture SceneTex;
texture BloomTex;

sampler SceneSampler = sampler_state
{
    Texture   = <SceneTex>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

sampler BloomSampler = sampler_state
{
    Texture   = <BloomTex>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

struct VS_INPUT  { float4 Position : POSITION0; float2 TexCoord : TEXCOORD0; };
struct VS_OUTPUT { float4 Position : POSITION0; float2 TexCoord : TEXCOORD0; };

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;
    o.Position = input.Position;
    o.TexCoord = input.TexCoord;
    return o;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float2 uv    = input.TexCoord;
    float4 scene = tex2D(SceneSampler, uv);
    float4 bloom = tex2D(BloomSampler, uv);   // black when BloomTex unset — safe to add

    // Additive bloom composite
    float4 result = scene + bloom * 0.8;

    // Subtle time-pulsed vignette — keeps Time from being stripped by compiler
    float2 centered  = uv - 0.5;
    float  vignette  = 1.0 - dot(centered, centered) * (1.2 + 0.1 * sin(Time * 0.5));
    result.rgb      *= saturate(vignette);

    return result;
}

technique PostProcess
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}
