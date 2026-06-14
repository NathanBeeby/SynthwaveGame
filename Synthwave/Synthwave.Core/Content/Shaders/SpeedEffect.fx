// SpeedEffect.fx
// Radial motion blur from screen centre, scales with vehicle speed.
//
// Fixes vs previous version:
//   1. SceneTex declared as a named texture + sampler so Parameters["SceneTex"]
//      works from C#.  register(s0)-only binding has no named parameter to set.
//   2. Time is actually used (drives vignette pulse) so the compiler keeps it
//      and Parameters["Time"] returns a valid EffectParameter instead of null.

float SpeedAmount;   // 0–1  (vehicle speed / max speed)
float Time;          // total elapsed seconds — used for vignette pulse

texture SceneTex;    // set via _speedEffect.Parameters["SceneTex"].SetValue(...)

sampler2D SceneSampler = sampler_state
{
    Texture   = <SceneTex>;
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
    float2 uv = input.TexCoord;

    [branch]
    if (SpeedAmount < 0.01)
        return tex2D(SceneSampler, uv);

    float2 centre   = float2(0.5, 0.5);
    float2 dir      = uv - centre;
    float  edgeDist = length(dir);
    float  blurPower = SpeedAmount * edgeDist * 0.18;

    // 8-tap radial zoom blur
    float4 color  = 0;
    float  weight = 0;

    for (int i = 0; i < 8; i++)
    {
        float t         = (float)i / 7.0;
        float scale     = 1.0 - blurPower * (1.0 - t);
        float2 sampleUV = saturate(centre + dir * scale);
        float  w        = 1.0 - t * 0.5;
        color  += tex2D(SceneSampler, sampleUV) * w;
        weight += w;
    }

    color /= weight;

    // Neon cyan edge streaks
    float vignetteStr  = pow(edgeDist, 2.0) * SpeedAmount * 0.6;
    color.rgb += float3(0.0, 0.9, 1.0) * vignetteStr;

    // Time-pulsed edge darkening — Time is used here so the compiler
    // retains the parameter and Parameters["Time"].SetValue() won't throw
    float pulse    = 0.5 + 0.5 * sin(Time * 8.0);   // fast flicker at speed
    float darkEdge = pow(edgeDist, 3.0) * SpeedAmount * (0.3 + pulse * 0.15);
    color.rgb     *= 1.0 - darkEdge;

    return color;
}

technique SpeedFX
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}
