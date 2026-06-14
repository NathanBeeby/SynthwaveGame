using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Menus.Core;

public class AnimatedCutscene(List<Texture2D> frames, float frameTime) : Cutscene
{
    #region Properties
    private readonly List<Texture2D> _frames = frames;
    private float _frameTime = frameTime;
    private float _timer;
    private int _index;
    #endregion

    #region Methods
    public override void Update(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= _frameTime)
        {
            _timer = 0;
            _index++;

            if (_index >= _frames.Count)
                IsFinished = true;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_index < _frames.Count)
            spriteBatch.Draw(_frames[_index], Vector2.Zero, Color.White);
    }
    #endregion
}
