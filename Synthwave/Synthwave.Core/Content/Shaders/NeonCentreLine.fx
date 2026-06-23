float4x4 World;
float4x4 View;
float4x4 Projection;

float Time;

float3 LineColor = float3(0.0, 1.0, 1.0); // neon cyan

float GlowIntensity = 8.0;
float PulseSpeed = 2.5;
float PulseAmount = 0.2;

struct VS_INPUT
{
    float3 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2 UV       : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;

    float4 world = mul(float4(input.Position,1), World);
    float4 view  = mul(world, View);
    o.Position   = mul(view, Projection);

    o.UV = input.UV;
    return o;
}

// edge fade based on UV.y (stable strip fade)
float Beam(float y)
{
    float d = abs(y - 0.5);
    return smoothstep(0.5, 0.0, d);
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float edge = abs(input.UV.x - 0.5);

    float glow = smoothstep(0.5, 0.0, edge);

    float pulse = 1.0 + sin(Time * 2.5) * 0.2;

    float3 color = float3(0.0, 1.0, 1.0) * glow * 8.0 * pulse;

    return float4(color, glow);
}
technique NeonCentreLine
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}