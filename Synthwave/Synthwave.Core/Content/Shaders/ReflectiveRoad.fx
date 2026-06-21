// ━━━━━━ Kernel: Pure Mirror Reflective Road ━━━━━━
// Does ONE job: a tinted mirror surface. No kerb, no centre line —
// those live in NeonSidewalk.fx and NeonCentreLine.fx on their own meshes.

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CameraPosition;
float  Time;

// 0 = matte road, 1 = full mirror. Drive this from weather (rain) or just
// hold it high (e.g. 0.7-0.9) for an always-glossy synthwave look.
float  Wetness = 0.85f;

// How much black is mixed into the reflection. 0 = pure mirror, 1 = solid
// black. Keep this low (0.10-0.18) — "tinted, not painted".
float  TintAmount = 0.12f;

// ─────────────────────────────────────────────────────────────
// Optional: real scene reflection (cars, buildings) instead of sky-only.
// Leave ReflectionStrength at 0 and this whole block is inert — the
// shader falls back to the procedural sky mirror below.
//
// To wire it up for real: render Traffic + City BEFORE the road pass,
// resolve that snapshot into a separate texture (you can't sample the
// same render target you're currently writing to), and set it here as
// ReflectionTex. It's a screen-space sample, flipped vertically, with a
// tiny normal-based wobble so it doesn't read as a flat decal.
// ─────────────────────────────────────────────────────────────
texture ReflectionTex;
sampler ReflectionSampler = sampler_state
{
    Texture = <ReflectionTex>;
    MinFilter = Linear; MagFilter = Linear; MipFilter = Linear;
    AddressU = Clamp; AddressV = Clamp;
};
float ReflectionStrength = 0.0f;

// ─────────────────────────────────────────────────────────────
// Vertex
// ─────────────────────────────────────────────────────────────
struct VS_INPUT
{
    float3 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 UV       : TEXCOORD0;
    float  RoadType : TEXCOORD1;
};

struct VS_OUTPUT
{
    float4 Position  : POSITION0;
    float3 WorldPos  : TEXCOORD0;
    float3 Normal    : TEXCOORD1;
    float4 ScreenPos : TEXCOORD2;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;

    float4 world = mul(float4(input.Position, 1.0), World);
    o.WorldPos = world.xyz;

    float4 view = mul(world, View);
    o.Position  = mul(view, Projection);
    o.ScreenPos = o.Position;

    float3x3 normalMatrix = (float3x3)World;
    o.Normal = normalize(mul(input.Normal, normalMatrix));

    return o;
}

// ─────────────────────────────────────────────────────────────
// Procedural sky (always-available reflection source)
// ─────────────────────────────────────────────────────────────
float3 SampleSky(float3 dir)
{
    float h = saturate(dir.y * 0.5 + 0.5);

    float3 horizon = float3(1.0, 0.4, 0.6);
    float3 top     = float3(0.55, 0.1, 0.65);

    float3 col = lerp(horizon, top, h);

    float band = sin(dir.x * 12.0 + Time * 0.4) * 0.5 + 0.5;
    col += saturate(band - 0.8) * horizon;

    return saturate(col * 1.6);
}

// ─────────────────────────────────────────────────────────────
// Pixel
// ─────────────────────────────────────────────────────────────
float4 PS(VS_OUTPUT input) : COLOR0
{
    float3 viewDir = normalize(CameraPosition - input.WorldPos);
    float3 normal  = normalize(input.Normal);
    float3 reflDir = reflect(-viewDir, normal);

    float3 skyRef = SampleSky(reflDir);

    // Optional real-scene reflection blended in on top of the sky.
    float2 screenUV = (input.ScreenPos.xy / input.ScreenPos.w) * 0.5 + 0.5;
    screenUV.y = 1.0 - screenUV.y;
    screenUV += normal.xz * 0.02;

    float3 sceneRef  = tex2D(ReflectionSampler, screenUV).rgb;
    float3 reflection = lerp(skyRef, sceneRef, ReflectionStrength);

    // Mild grazing-angle darkening only — never breaks the mirror illusion.
    float ndv = saturate(dot(normal, viewDir));
    float angleFactor = lerp(0.85, 1.0, ndv);

    float3 mirror = reflection * Wetness * angleFactor;
    float3 col    = lerp(mirror, float3(0, 0, 0), TintAmount);

    return float4(saturate(col), 1.0);
}

technique ReflectiveRoad
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}
