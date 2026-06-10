namespace Synthwave.Core.Classes.Renderer;

public static class ShaderLibrary
{
    public static string Neon = @"
        float pulse = sin(time * 3.0) * 0.5 + 0.5;
        return baseColor * (intensity + pulse);
    ";

    public static string Fog = @"
        float f = exp(-distance * density);
        return lerp(fogColor, color, f);
    ";
}
