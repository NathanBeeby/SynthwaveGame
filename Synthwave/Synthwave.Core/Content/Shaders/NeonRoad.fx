// NeonRoad.fx
// Simple neon road shader compatible with SM 3.0

float4x4 World;
float4x4 View;
float4x4 Projection;

float Time;

float3 CenterColor = float3(0.1, 1.0, 1.0);   // neon cyan
float3 KerbColor   = float3(1.0, 0.6, 0.1);   // orange/yellow

float CenterWidth = 0.08;
float KerbWidth = 0.15;

struct VS_IN
{
    float4 Pos : POSITION0;
    float3 Normal : NORMAL0;
    float2 UV : TEXCOORD0;
};

struct PS_IN
{
    float4 Pos : POSITION0;
    float2 UV : TEXCOORD0;
};

// Vertex shader
PS_IN VS(VS_IN input)
{
    PS_IN output;

    float4 world = mul(input.Pos, World);
    float4 view = mul(world, View);
    output.Pos = mul(view, Projection);

    output.UV = input.UV;
    return output;
}

// Pixel shader
float4 PS(PS_IN input) : COLOR
{
    float x = input.UV.x;

    float3 col = float3(0.05, 0.05, 0.05); // asphalt base

    // center neon strip
    float centerMask = 1.0 - smoothstep(CenterWidth, CenterWidth + 0.02, abs(x - 0.5));
    col += CenterColor * centerMask * 3.0;

    // kerbs (edges)
    float leftKerb  = 1.0 - smoothstep(KerbWidth, KerbWidth + 0.03, x);
    float rightKerb = 1.0 - smoothstep(KerbWidth, KerbWidth + 0.03, 1.0 - x);

    col += KerbColor * (leftKerb + rightKerb) * 2.5;

    return float4(col, 1);
}

// Technique
technique Technique1
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}