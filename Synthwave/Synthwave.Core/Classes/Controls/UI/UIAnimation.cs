using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Controls.UI;

public class UIAnimation
{
    public Vector2 Position;
    public Vector2 Target;

    public float Speed = 10f;

    public void Update(float dt)
    {
        Position = Vector2.Lerp(Position, Target, dt * Speed);
    }
}