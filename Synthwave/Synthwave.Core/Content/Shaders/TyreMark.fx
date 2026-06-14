// Tyre skid mark shader
float Fade = 1.0;           // 1 = full intensity, 0 = invisible
texture SceneTex;           // The road scene
texture SkidMapTex;         // Skid marks texture (render target)

sampler SceneSampler = sampler_state
{
    Texture = <SceneTex>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
};

sampler SkidSampler = sampler_state
{
    Texture = <SkidMapTex>;
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

// ---------------- Pixel Shader ----------------
float4 PS(VS_OUTPUT input) : COLOR0
{
    float4 roadCol = tex2D(SceneSampler, input.TexCoord);
    float4 skidCol = tex2D(SkidSampler, input.TexCoord);

    // multiply skid intensity by fade
    skidCol.rgb *= Fade;

    // darken road where skid marks exist
    float3 finalCol = lerp(roadCol.rgb, skidCol.rgb, skidCol.a);

    return float4(finalCol, 1.0);
}

// ---------------- Technique ----------------
technique TyreMarks
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}