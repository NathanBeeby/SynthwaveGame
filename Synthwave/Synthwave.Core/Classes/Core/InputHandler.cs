using Microsoft.Xna.Framework.Input;

namespace Synthwave.Core.Classes.Core;

public class InputHandler
{
    #region Properties
    private KeyboardState _current;
    private KeyboardState _previous;
    #endregion

    #region Methods
    public void Update()
    {
        _previous = _current;
        _current = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key) => _current.IsKeyDown(key);
    public bool IsKeyUp(Keys key) => _current.IsKeyUp(key);
    public bool WasJustPressed(Keys key) => _current.IsKeyDown(key) && _previous.IsKeyUp(key);
    #endregion
}