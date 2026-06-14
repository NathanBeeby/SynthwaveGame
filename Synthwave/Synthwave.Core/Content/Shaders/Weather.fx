// Weather.fx
// Composites rain streaks, snowfall, and distance fog over the scene texture.
// Parameters set by WeatherSystem.ApplyToEffect / SynthwaveWorld.Draw

sampler2D SceneSampler : register(s0);

float RainAmount;   // 0–1
float SnowAmount;   // 0–1
float FogDensity;   // 0–1  (1 - Visibility)
float Time;

// ── Helpers ──────────────────────────────────────────────────────────────────

float Hash(float2 p)
{
    return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

float Hash2(float2 p)
{
    return frac(sin(dot(p, float2(269.5, 183.3))) * 43758.5453);
}

// ── Rain ─────────────────────────────────────────────────────────────────────
// Vertical streaks that scroll downward; more streaks = higher RainAmount.

float RainLayer(float2 uv, float speed, float density, float width, float len)
{
    // tile UV into small cells
    float2 cell = floor(uv * float2(density, density * 0.25));
    float  rand  = Hash(cell);

    float2 localUV = frac(uv * float2(density, density * 0.25));

    // horizontal position of streak inside cell
    float xPos   = rand;
    float xDist  = abs(localUV.x - xPos);

    // time-offset per cell so not all streaks move together
    float yOff   = frac(Time * speed + rand);
    float yDist  = abs(frac(localUV.y + yOff) - 0.5);

    float streak = smoothstep(width, 0.0, xDist) *
                   smoothstep(len,  0.0, yDist);
    return streak;
}

float4 ApplyRain(float4 color, float2 uv)
{
    if (RainAmount < 0.01) return color;

    float rain = 0;
    // Three layers at different scales / speeds give depth
    rain += RainLayer(uv, 1.8, 80,  0.004, 0.04) * 0.6;
    rain += RainLayer(uv, 2.4, 120, 0.003, 0.03) * 0.4;
    rain += RainLayer(uv, 1.2, 50,  0.006, 0.06) * 0.3;

    rain = saturate(rain * RainAmount);

    // Slight blue-grey tint + streak brightness
    float3 rainColor = float3(0.6, 0.7, 0.9);
    color.rgb = lerp(color.rgb, rainColor, rain * 0.55);
    color.rgb += rainColor * rain * 0.3;

    return color;
}

// ── Snow ─────────────────────────────────────────────────────────────────────
// Soft circular flakes that drift and fall.

float SnowFlakes(float2 uv, float scale, float speed)
{
    float2 scaled = uv * scale;
    float2 cell   = floor(scaled);
    float2 local  = frac(scaled);

    float result = 0;

    // Check a 3x3 neighbourhood so flakes near cell edges are visible
    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            float2 neighbour = float2(x, y);
            float2 c         = cell + neighbour;

            float r1 = Hash(c);
            float r2 = Hash(c + float2(13.7, 91.3));

            // Drift + fall
            float2 offset = float2(
                r1 * 0.6 - 0.3 + sin(Time * 0.4 + r1 * 6.28) * 0.12,
                frac(-Time * (0.15 + r2 * 0.1))
            );

            float2 flakeUV = local - neighbour - offset;
            float  dist    = length(flakeUV);
            float  radius  = 0.06 + r1 * 0.06;

            result += smoothstep(radius, radius * 0.4, dist);
        }
    }

    return saturate(result);
}

float4 ApplySnow(float4 color, float2 uv)
{
    if (SnowAmount < 0.01) return color;

    float snow = 0;
    snow += SnowFlakes(uv, 18.0, 0.18) * 0.7;
    snow += SnowFlakes(uv, 30.0, 0.25) * 0.4;
    snow += SnowFlakes(uv, 10.0, 0.12) * 0.5;

    snow = saturate(snow * SnowAmount);

    // Snow flakes are bright white with a faint blue tint
    float3 snowColor = float3(0.92, 0.96, 1.0);
    color.rgb = lerp(color.rgb, snowColor, snow * 0.8);

    return color;
}

// ── Fog ──────────────────────────────────────────────────────────────────────
// Screen-space fog darkens and desaturates toward synthwave purple.

float4 ApplyFog(float4 color, float2 uv)
{
    if (FogDensity < 0.01) return color;

    // Fog is denser toward the horizon (top half of screen)
    float horizonFactor = smoothstep(0.3, 0.7, 1.0 - uv.y);
    float fogAmount     = FogDensity * horizonFactor;

    // Synthwave fog colour: deep purple
    float3 fogColor = float3(0.08, 0.0, 0.18);
    color.rgb = lerp(color.rgb, fogColor, saturate(fogAmount * 1.2));

    return color;
}

// ── VS / PS ───────────────────────────────────────────────────────────────────

struct VSInput  { float4 Position : POSITION; float2 TexCoord : TEXCOORD0; };
struct PSInput  { float4 Position : SV_POSITION; float2 TexCoord : TEXCOORD0; };

PSInput VS(VSInput input)
{
    PSInput output;
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(PSInput input) : COLOR
{
    float2 uv    = input.TexCoord;
    float4 color = tex2D(SceneSampler, uv);

    color = ApplyRain(color, uv);
    color = ApplySnow(color, uv);
    color = ApplyFog(color, uv);

    return color;
}

technique WeatherFX
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}
