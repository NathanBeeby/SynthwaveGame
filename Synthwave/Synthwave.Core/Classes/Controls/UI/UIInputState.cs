using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Synthwave.Core.Classes.Controls.UI;

public class UIInputState
{
    public Vector2 MousePosition;
    public bool MousePressed;
    public bool MouseDown;

    public Vector2 LeftStick;
    public bool ConfirmPressed;
    public bool BackPressed;

    public KeyboardState Keyboard;
    public GamePadState GamePad;
}
