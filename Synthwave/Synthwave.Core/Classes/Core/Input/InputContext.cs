using Microsoft.Xna.Framework.Input;

namespace Synthwave.Core.Classes.Core.Input;

public class InputContext
{
    public KeyboardState Keyboard;
    public MouseState Mouse;
    public GamePadState GamePad;

    public bool IsBlocked;
}
