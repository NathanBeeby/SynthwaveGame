using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Synthwave.Core.Classes.Core.Input;

public class InputManager
{
    private InputContext _current = new();

    public void Update()
    {
        _current.Keyboard = Keyboard.GetState();
        _current.Mouse = Mouse.GetState();
        _current.GamePad = GamePad.GetState(PlayerIndex.One);
    }

    public InputContext GetState() => _current;

    public void BlockInput(bool block)
    {
        _current.IsBlocked = block;
    }
}
