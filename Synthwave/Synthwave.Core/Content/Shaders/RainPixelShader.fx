// RainPixelShader.fx  –  ps_3_0
//
// Upgrade from ps_2_0 was required: the fixed streak logic needs ~246 arithmetic
// slots; ps_2_0 allows only 64.  ps_3_0 raises the cap to 512 and adds free
// flow-control, so the early-out and the per-layer wind work without workarounds.
//
// Instruction-budget changes vs the broken ps_2_0 attempt:
//   • hash() replaced with a trig-free integer-style multiply (saves ~8 slots/call)
//   • Layers reduced from 3 to 2  (each layer ~35 slots; 2 × 35 = 70, well under 512)
//   • sin(WindAngle) computed once in PS, not inside each layer
//   • Early-out via [branch] on RainIntensity

float RainIntensity;   // 0–1, set by WeatherSystem
float Time;
float WindAngle;       // radians; 0 = straight down, positive = slant right

texture SceneTex;

sampler2D SceneSampler = sampler_state
{
    Texture   = <SceneTex>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

struct VS_INPUT  { float4 Position : POSITION0; float2 TexCoord : TEXCOORD0; };
struct VS_OUTPUT { float4 Position : POSITION0; float2 TexCoord : TEXCOORD0; };

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT o;
    o.Position = input.Position;
    o.TexCoord = input.TexCoord;
    return o;
}

// ── Trig-free hash ────────────────────────────────────────────────────────────
// Uses only frac + dot with large primes.  ~3 slots vs ~8 for sin-based hash.
float hash(float2 p)
{
    p = frac(p * float2(443.8975, 397.2973));
    p += dot(p, p + 19.19);
    return frac(p.x * p.y);
}

// ── Single rain layer ─────────────────────────────────────────────────────────
// uv    : pre-scaled screen UV
// speed : fall speed
// wind  : pre-computed horizontal drift (sin(WindAngle) * 0.15), passed in once
float rainLayer(float2 uv, float speed, float wind)
{
    uv.x += Time * wind;

    float2 id = floor(uv);
    float2 gv = frac(uv);

    float rCol   = hash(float2(id.x, 0.0));
    float rSpeed = hash(float2(id.x, 1.0));
    float rAlpha = hash(float2(id.x, 2.0));

    float scrollY = frac(uv.y * 0.05 - Time * (speed + rSpeed * 0.5));

    float xDist      = abs(gv.x - rCol);
    float streak     = smoothstep(0.045, 0.0, xDist);
    float streakBody = smoothstep(1.0, 0.6, scrollY);
    float streakTail = smoothstep(0.0, 0.3, scrollY);

    return streak * streakBody * streakTail * (0.5 + rAlpha * 0.5);
}

// ── Pixel shader ──────────────────────────────────────────────────────────────
float4 PS(VS_OUTPUT input) : COLOR0
{
    float2 uv  = input.TexCoord;
    float4 col = tex2D(SceneSampler, uv);

    // [branch] lets the GPU skip all streak work on dry frames at no extra cost
    [branch]
    if (RainIntensity < 0.01)
        return col;

    // Compute wind drift once — not inside each layer
    float wind = sin(WindAngle) * 0.15;

    // Two layers instead of three keeps us well inside the 512-slot budget
    float rain = 0;
    rain += rainLayer(uv * float2(55.0, 18.0), 1.8, wind * 1.2) * 0.6;   // fine/fast
    rain += rainLayer(uv * float2(28.0, 10.0), 1.2, wind * 0.7) * 0.4;   // coarse/slow

    rain  = saturate(rain) * RainIntensity;

    float3 streakColor = float3(0.65, 0.75, 1.0);
    col.rgb += streakColor * rain * 0.5;

    // Slight overall blue-grey cast in heavy rain
    col.rgb = lerp(col.rgb, float3(0.55, 0.60, 0.70), RainIntensity * 0.18);

    return col;
}

technique Rain
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader  = compile ps_3_0 PS();
    }
}
