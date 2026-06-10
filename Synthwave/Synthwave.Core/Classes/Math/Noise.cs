using System;

namespace Synthwave.Core.Classes.Math;

public static class Noise
{
    private static readonly int[] perm;

    static Noise()
    {
        perm = new int[512];
        Random r = new(1337);
        int[] p = new int[256];

        for (int i = 0; i < 256; i++) p[i] = i;

        for (int i = 0; i < 256; i++)
        {
            int swap = r.Next(256);
            (p[i], p[swap]) = (p[swap], p[i]);
        }

        for (int i = 0; i < 512; i++)
            perm[i] = p[i & 255];
    }

    public static float Perlin(float x, float y)
    {
        int xi = (int)x & 255;
        int yi = (int)y & 255;

        float xf = x - (int)x;
        float yf = y - (int)y;

        float u = Fade(xf);
        float v = Fade(yf);

        int aa = perm[xi + perm[yi]];
        int ab = perm[xi + perm[yi + 1]];
        int ba = perm[xi + 1 + perm[yi]];
        int bb = perm[xi + 1 + perm[yi + 1]];

        float x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        float x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

        return (Lerp(x1, x2, v) + 1) / 2;
    }

    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    private static float Lerp(float a, float b, float t) => a + t * (b - a);

    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 3;
        return h switch
        {
            0 => x + y,
            1 => -x + y,
            2 => x - y,
            _ => -x - y
        };
    }
}
