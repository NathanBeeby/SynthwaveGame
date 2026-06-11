using System;

namespace Synthwave.Core.Classes.Core;

internal static class RandomExt
{
    public static float NextSingle(this Random r, float min, float max)
        => min + (float)r.NextDouble() * (max - min);
}
