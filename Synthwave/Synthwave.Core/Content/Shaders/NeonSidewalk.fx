float4x4 World;
float4x4 View;
float4x4 Projection;

float Time;

float3 LineColor = float3(1.0, 0.65, 0.0); // neon orange/yellow
float GlowIntensity = 6.0;

float GridScale = 3.0;
float LineWidth = 0.06;

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

    o.UV = input.UV * GridScale;
    return o;
}

// crisp grid line function (IMPORTANT FIX)
float GridLine(float coord)
{
    float f = frac(coord);
    float d = min(f, 1.0 - f);
    return smoothstep(LineWidth, 0.0, d);
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float2 uv = input.UV;

    float gx = frac(uv.x);
    float gy = frac(uv.y);

    float dx = min(gx, 1.0 - gx);
    float dy = min(gy, 1.0 - gy);

    float lineX = smoothstep(0.08, 0.0, dx);
    float lineY = smoothstep(0.08, 0.0, dy);

    float grid = max(lineX, lineY);

    float3 color = float3(1.0, 0.65, 0.0) * grid * 6.0;

    return float4(color, grid);
}
technique NeonSidewalk
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}