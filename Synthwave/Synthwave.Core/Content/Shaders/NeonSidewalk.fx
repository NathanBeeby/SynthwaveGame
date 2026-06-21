// ━━━━━━ Kernel: Neon Square-Panel Sidewalk ━━━━━━
// Runs on the kerb/sidewalk strips (both sides of the road). UV.x and UV.y
// are both real world-space distances (not normalised 0..1), so TileSize
// below is literally "metres per panel" and the seams stay square no
// matter how long the road segment is.

float4x4 World;
float4x4 View;
float4x4 Projection;

float  Time;
float3 TileLineColor = float3(1.0, 0.85, 0.0); // neon yellow
float3 PanelColor    = float3(0.05, 0.04, 0.02); // dark slab fill between seams
float  TileSize      = 3.0f;   // world units per square panel
float  LineThickness = 0.06f;  // fraction of a tile taken by the glowing seam
float  GlowIntensity = 2.0f;

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

    o.UV = input.UV;

    return o;
}

// Distance (0..1 of a tile) to the nearest seam along one axis.
float SeamMask(float worldCoord)
{
    float cell = frac(worldCoord / TileSize);
    float d = min(cell, 1.0 - cell);
    return 1.0 - smoothstep(0.0, LineThickness, d);
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float seamX = SeamMask(input.UV.x);
    float seamY = SeamMask(input.UV.y);
    float seam  = saturate(seamX + seamY);

    float3 col = lerp(PanelColor, TileLineColor * GlowIntensity, seam);

    return float4(saturate(col), 1.0);
}

technique NeonSidewalk
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}
