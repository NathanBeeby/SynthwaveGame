float Time;
float RainIntensity;
float2 WindDirection;

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
    VS_OUTPUT output;
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float4 col = tex2D(SceneSampler, input.TexCoord);

    // The "sky" band sits at the TOP of the frame (TexCoord.y near 0).
    // Push an overcast tint into that band as rain ramps up, fading to
    // nothing by mid-screen so the ground/road/traffic are never erased.
    float skyMask  = saturate(1.0 - input.TexCoord.y * 2.0);
    float overcast = saturate(skyMask * RainIntensity) * 0.6;

    float3 sky = float3(0.45, 0.5, 0.65);

    col.rgb = lerp(col.rgb, sky, overcast);

    return col;
}

technique RainSky
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}
