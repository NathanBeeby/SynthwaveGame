// ━━━━━━ Kernel: Neon Dashed Centre Line ━━━━━━
// The dashing is NOT done here — NeonRoadMeshBuilder.BuildCentreLine only
// emits geometry for the "on" segments of the dash pattern, so this shader
// has nothing to discard. It just renders a soft neon glow across the
// strip's width and gives it a gentle pulse.

float4x4 World;
float4x4 View;
float4x4 Projection;

float  Time;
float3 LineColor    = float3(0.0, 1.0, 1.0); // neon cyan
float  GlowIntensity = 2.5f;
float  PulseSpeed    = 2.0f;
float  PulseAmount   = 0.15f; // how much the glow breathes, 0 = static

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
    float2 UV       : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;

    float4 world = mul(float4(input.Position, 1.0), World);
    float4 view  = mul(world, View);
    o.Position   = mul(view, Projection);

    o.UV = input.UV; // x: 0..1 across strip width, y: distance along (unused here)

    return o;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    // Bright core fading to the edges of the strip — gives it a soft
    // neon-tube look rather than a flat painted line.
    float distFromCentre = abs(input.UV.x - 0.5) * 2.0; // 0 centre -> 1 edge
    float core = 1.0 - smoothstep(0.0, 1.0, distFromCentre);

    float pulse = 1.0 + sin(Time * PulseSpeed) * PulseAmount;

    float3 col = LineColor * core * GlowIntensity * pulse;

    // Alpha follows the glow falloff too, so use additive/alpha blending
    // on the C# side (BlendState.Additive looks best for neon).
    return float4(saturate(col), core);
}

technique NeonCentreLine
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}
