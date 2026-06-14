using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using System;

namespace Synthwave.Core.Classes.Controls.UI;

public class UITextBox : UIElement
{
    public string Text = "";
    public bool IsFocused;

    public Action<string> OnChanged;

    private double _blinkTime;

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var keyboard = Keyboard.GetState();

        var bounds = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y);

        if (mouse.LeftButton == ButtonState.Pressed &&
            bounds.Contains(mouse.Position))
        {
            IsFocused = true;
        }
        else if (mouse.LeftButton == ButtonState.Pressed)
        {
            IsFocused = false;
        }

        if (!IsFocused)
            return;

        foreach (var key in keyboard.GetPressedKeys())
        {
            char c = KeyToChar(key);

            if (c != '\0' && !Text.EndsWith(c))
            {
                Text += c;
                OnChanged?.Invoke(Text);
            }
        }

        if (keyboard.IsKeyDown(Keys.Back) && Text.Length > 0)
        {
            Text = Text[..^1];
            OnChanged?.Invoke(Text);
        }

        _blinkTime += gameTime.ElapsedGameTime.TotalSeconds;
    }

    private static char KeyToChar(Keys key)
    {
        return key switch
        {
            Keys.A => 'a',
            Keys.B => 'b',
            Keys.C => 'c',
            Keys.D => 'd',
            Keys.E => 'e',
            Keys.F => 'f',
            Keys.G => 'g',
            Keys.H => 'h',
            Keys.I => 'i',
            Keys.J => 'j',
            Keys.K => 'k',
            Keys.L => 'l',
            Keys.M => 'm',
            Keys.N => 'n',
            Keys.O => 'o',
            Keys.P => 'p',
            Keys.Q => 'q',
            Keys.R => 'r',
            Keys.S => 's',
            Keys.T => 't',
            Keys.U => 'u',
            Keys.V => 'v',
            Keys.W => 'w',
            Keys.X => 'x',
            Keys.Y => 'y',
            Keys.Z => 'z',
            Keys.Space => ' ',
            _ => '\0'
        };
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // background
         spriteBatch.Draw(TextureStore.Pixel, Position, IsFocused ? Color.White : Color.Gray);

        // text rendering handled elsewhere (SpriteFont)
    }
}