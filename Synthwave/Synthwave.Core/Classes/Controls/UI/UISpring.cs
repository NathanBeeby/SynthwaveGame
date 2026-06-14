using Microsoft.Xna.Framework;
using System;

namespace Synthwave.Core.Classes.Controls.UI;

public static class UISpring
{
    public static Vector2 Apply(Vector2 current, Vector2 target, float speed, float dt)
    {
        return current + (target - current) * (1f - MathF.Exp(-speed * dt));
    }
}
