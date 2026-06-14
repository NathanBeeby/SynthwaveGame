// FogShader.fx
// Animated screen-space fog with a synthwave neon colour palette.
// Layered noise gives the fog organic movement.

texture SceneTex;

sampler2D SceneSampler = sampler_state
{
    Texture   = <SceneTex>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

float FogIntensity;  // 0–1
float Time;

// ── Noise ─────────────────────────────────────────────────────────────────────

float Hash(float2 p)
{
    return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

// Bilinear-interpolated value noise
float ValueNoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    float2 u = f * f * (3.0 - 2.0 * f); // smoothstep

    float a = Hash(i);
    float b = Hash(i + float2(1, 0));
    float c = Hash(i + float2(0, 1));
    float d = Hash(i + float2(1, 1));

    return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
}

// Two octaves of noise for convincing fog wisps
float FogNoise(float2 uv)
{
    float2 motion1 = float2(Time * 0.03, Time * 0.015);
    float2 motion2 = float2(-Time * 0.02, Time * 0.025);

    float n1 = ValueNoise(uv * 3.5 + motion1);
    float n2 = ValueNoise(uv * 7.0 + motion2);

    return n1 * 0.65 + n2 * 0.35;
}

// ── VS / PS ───────────────────────────────────────────────────────────────────

struct VSInput { float4 Position : POSITION; float2 TexCoord : TEXCOORD0; };
struct PSInput { float4 Position : SV_POSITION; float2 TexCoord : TEXCOORD0; };

PSInput VS(VSInput input)
{
    PSInput output;
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(PSInput input) : COLOR
{
    float2 uv    = input.TexCoord;
    float4 scene = tex2D(SceneSampler, uv);

    if (FogIntensity < 0.01) return scene;

    float heightGradient = pow(saturate(uv.y), 2.0);
    float noise          = FogNoise(uv);

    float fogAmount = FogIntensity * heightGradient * (0.5 + noise * 0.3);
    fogAmount       = saturate(fogAmount) * 0.85;

    float  cycle    = sin(Time * 0.25) * 0.5 + 0.5;
    float3 fogColor = lerp(
        float3(0.05, 0.0,  0.20),
        float3(0.30, 0.0,  0.35),
        cycle * 0.4
    );

    float3 result = lerp(scene.rgb, fogColor, fogAmount);
    result       += fogColor * fogAmount * 0.10;

    return float4(saturate(result), scene.a);
}

technique FogFX
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}