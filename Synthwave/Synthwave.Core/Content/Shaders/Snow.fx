float SnowAmount;
float Time;
float WindStrength;

texture SceneTex;

sampler2D SceneSampler = sampler_state
{
    Texture = <SceneTex>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
};

struct VS_INPUT
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;
    o.Position = input.Position;
    o.TexCoord = input.TexCoord;
    return o;
}

float hash(float2 p)
{
    return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float2 uv  = input.TexCoord;
    float4 col = tex2D(SceneSampler, uv);

    if (SnowAmount < 0.01) return col;

    // Scroll the cell grid downward (falling) and sideways (wind) over time
    // so the flake pattern actually moves instead of being a static dither.
    float2 scroll = float2(WindStrength * Time * 0.4, Time * 3.0);
    float2 cell   = floor(uv * 80.0 + scroll);

    float n = hash(cell);

    // Only a sparse set of cells light up as flakes; SnowAmount widens
    // the band of cells that qualify.
    float flake = step(1.0 - SnowAmount * 0.15, n);

    float3 c = float3(0.9, 0.95, 1.0);
    col.rgb = lerp(col.rgb, c, flake * 0.9);

    return col;
}

technique Snow
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}
