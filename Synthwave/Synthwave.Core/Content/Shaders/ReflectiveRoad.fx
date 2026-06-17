// ━━━━━━ Kernel: Pure Mirror Reflective Road ━━━━━━

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CameraPosition;
float  Time;
float  Wetness;          // 0 = dry, 1 = mirror wet
float  EmissiveBoost = 2.5f;

// ─────────────────────────────────────────────────────────────
// Colors
// ─────────────────────────────────────────────────────────────
float3 RoadColor       = float3(0.04, 0.03, 0.07);
float3 CentreLineColor = float3(0.0, 1.0, 1.0);
float3 KerbColor       = float3(1.0, 0.85, 0.0);

// UV layout
float KerbBandWidth       = 0.04f;
float CenterLineHalfWidth = 0.015f;

// Dash
float DashLength = 6.0;
float DashGap    = 4.0;

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
    float4 Position : POSITION0;
    float3 WorldPos : TEXCOORD0;
    float3 Normal   : TEXCOORD1;
    float2 UV       : TEXCOORD2;
    float  RoadType : TEXCOORD3;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;

    float4 world = mul(float4(input.Position, 1.0), World);
    o.WorldPos = world.xyz;

    float4 view = mul(world, View);
    o.Position = mul(view, Projection);

    float3x3 normalMatrix = (float3x3)World;
    o.Normal = normalize(mul(input.Normal, normalMatrix));

    o.UV = input.UV;
    o.RoadType = input.RoadType;

    return o;
}

// ─────────────────────────────────────────────────────────────
// Procedural Sky (reflection source)
// ─────────────────────────────────────────────────────────────
float3 SampleSky(float3 dir)
{
    float h = saturate(dir.y * 0.5 + 0.5);

    float3 horizon = float3(1.0, 0.4, 0.6);
    float3 top     = float3(0.55, 0.1, 0.65);

    float3 col = lerp(horizon, top, h);

    float band = sin(dir.x * 12.0 + Time * 0.4) * 0.5 + 0.5;
    col += saturate(band - 0.8) * horizon;

    // boost brightness so reflections read clearly
    return saturate(col * 1.6);
}

// ─────────────────────────────────────────────────────────────
// Pixel Shader
// ─────────────────────────────────────────────────────────────
float4 PS(VS_OUTPUT input) : COLOR0
{
    float u = saturate(input.UV.x);
    float distAlong = input.UV.y;

    bool isStreet = input.RoadType < 0.5f;

    float3 baseColor = RoadColor;
    float emissive = 0.0;

    // ─────────────────────────────────────────────
    // ROAD DETAILS (kerbs + center line)
    // ─────────────────────────────────────────────
    if (isStreet)
    {
        float leftKerb  = step(u, KerbBandWidth);
        float rightKerb = step(1.0 - KerbBandWidth, u);
        float kerbMask  = saturate(leftKerb + rightKerb);

        float distToCenter = abs(u - 0.5);

        float centerMask =
            1.0 - smoothstep(CenterLineHalfWidth,
                             CenterLineHalfWidth * 2.0,
                             distToCenter);

        float period = DashLength + DashGap;
        float phase  = frac(distAlong / period);
        float dashOn = step(phase, DashLength / period);

        centerMask *= dashOn;

        emissive += kerbMask * 1.5;
        emissive += centerMask * 2.0;

        // reduce albedo influence (important for mirror look)
        baseColor *= 0.25;
    }
    else
    {
        float radiusBand = smoothstep(0.48f, 0.5f, abs(u - 0.5f));
        emissive += radiusBand * 1.2;
    }

    // ─────────────────────────────────────────────
    // TRUE MIRROR REFLECTION MODEL
    // ─────────────────────────────────────────────
    float3 viewDir = normalize(CameraPosition - input.WorldPos);
    float3 normal  = normalize(input.Normal);

    float3 reflDir = reflect(-viewDir, normal);
    float3 skyRef  = SampleSky(reflDir);

    // wetness is DIRECT mirror strength (no Fresnel dependency)
    float mirrorStrength = Wetness;

    // slight angular darkening only (not Fresnel blending)
    float ndv = saturate(dot(normal, viewDir));
    float angleFactor = lerp(0.85, 1.0, ndv);

    // base becomes nearly black when wet
    float3 dryRoad = RoadColor;
    float3 wetRoad = float3(0.01, 0.01, 0.015);

    float3 base = lerp(dryRoad, wetRoad, Wetness);

    // FINAL COMPOSITION (reflection-first)
    float3 col = skyRef * mirrorStrength * angleFactor
               + base * (1.0 - mirrorStrength);

    // ─────────────────────────────────────────────
    // EMISSIVE OVERLAY (after reflection)
    // ─────────────────────────────────────────────
    float3 emissiveCol = col * emissive * EmissiveBoost;
    col += saturate(emissiveCol);

    return float4(col, 1.0);
}

// ─────────────────────────────────────────────────────────────
technique ReflectiveRoad
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}