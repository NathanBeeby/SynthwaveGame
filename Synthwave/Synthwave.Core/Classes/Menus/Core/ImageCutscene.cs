using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Menus.Core;

public class ImageCutscene(Texture2D image, float duration) : Cutscene
{
    #region Properties
    private Texture2D _image = image;
    private float _duration = duration;
    private float _timer;

    #endregion

    #region Methods
    public override void Update(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= _duration)
            IsFinished = true;
    }

    public override void Draw(SpriteBatch spriteBatch) => spriteBatch.Draw(_image, Vector2.Zero, Color.White);
    #endregion
}