using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Synthwave.Core.Classes.Controls.UI;

public class UIInput
{
    public UIInputState State;

    public void Update()
    {
        var mouse = Mouse.GetState();
        var pad = GamePad.GetState(PlayerIndex.One);
        var keyboard = Keyboard.GetState();

        State = new UIInputState
        {
            MousePosition = mouse.Position.ToVector2(),
            MousePressed = mouse.LeftButton == ButtonState.Pressed,
            MouseDown = mouse.LeftButton == ButtonState.Pressed,

            LeftStick = pad.ThumbSticks.Left,
            ConfirmPressed = pad.Buttons.A == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Enter),
            BackPressed = pad.Buttons.B == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape),

            Keyboard = keyboard,
            GamePad = pad
        };
    }
}