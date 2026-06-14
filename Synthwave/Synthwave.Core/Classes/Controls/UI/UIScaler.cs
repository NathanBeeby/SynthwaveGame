using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Controls.UI;

public static class UIScaler
{
    public static Vector2 ScreenSize;
    public static Vector2 ReferenceSize = new(1920, 1080);

    public static Vector2 Scale(Vector2 pos) => new(pos.X / ReferenceSize.X * ScreenSize.X,pos.Y / ReferenceSize.Y * ScreenSize.Y);
}
