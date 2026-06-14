using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Particles;

public class BloomManager
{
    private GraphicsDevice _graphics;

    private RenderTarget2D _sceneTarget;
    private RenderTarget2D _brightTarget;
    private RenderTarget2D _blurX;
    private RenderTarget2D _blurY;

    private Effect _brightPassEffect;
    private Effect _blurEffect;
    private Effect _combineEffect;

    public float Threshold = 0.8f;
    public float Intensity = 1.2f;

    public BloomManager(GraphicsDevice graphics,
                    Effect brightPass,
                    Effect blur,
                    Effect combine)
    {
        _graphics = graphics;

        _brightPassEffect = brightPass;
        _blurEffect = blur;
        _combineEffect = combine;

        var pp = graphics.PresentationParameters;

        _sceneTarget = new RenderTarget2D(graphics, pp.BackBufferWidth, pp.BackBufferHeight);
        _brightTarget = new RenderTarget2D(graphics, pp.BackBufferWidth, pp.BackBufferHeight);
        _blurX = new RenderTarget2D(graphics, pp.BackBufferWidth, pp.BackBufferHeight);
        _blurY = new RenderTarget2D(graphics, pp.BackBufferWidth, pp.BackBufferHeight);
    }

    public RenderTarget2D BeginScene()
    {
        _graphics.SetRenderTarget(_sceneTarget);
        _graphics.Clear(Color.Black);

        return _sceneTarget;
    }

    public RenderTarget2D EndScene()
    {
        _graphics.SetRenderTarget(null);
        return _sceneTarget;
    }

    public void ExtractBrightPass(SpriteBatch spriteBatch)
    {
        _graphics.SetRenderTarget(_brightTarget);
        _graphics.Clear(Color.Black);

        _brightPassEffect.Parameters["Threshold"]?.SetValue(Threshold);

        spriteBatch.Begin(effect: _brightPassEffect);
        spriteBatch.Draw(_sceneTarget, Vector2.Zero, Color.White);
        spriteBatch.End();
    }

    public void BlurHorizontal(SpriteBatch spriteBatch)
    {
        _graphics.SetRenderTarget(_blurX);
        _graphics.Clear(Color.Black);

        _blurEffect.Parameters["Direction"]?.SetValue(new Vector2(1, 0));

        spriteBatch.Begin(effect: _blurEffect);
        spriteBatch.Draw(_brightTarget, Vector2.Zero, Color.White);
        spriteBatch.End();
    }

    public void BlurVertical(SpriteBatch spriteBatch)
    {
        _graphics.SetRenderTarget(_blurY);
        _graphics.Clear(Color.Black);

        _blurEffect.Parameters["Direction"]?.SetValue(new Vector2(0, 1));

        spriteBatch.Begin(effect: _blurEffect);
        spriteBatch.Draw(_blurX, Vector2.Zero, Color.White);
        spriteBatch.End();
    }

    public void Combine(SpriteBatch spriteBatch)
    {
        _graphics.SetRenderTarget(null);

        _graphics.Clear(Color.Black);

        _combineEffect.Parameters["BloomIntensity"]?.SetValue(Intensity);

        spriteBatch.Begin(effect: _combineEffect);

        // base scene
        spriteBatch.Draw(_sceneTarget, Vector2.Zero, Color.White);

        // bloom overlay
        spriteBatch.Draw(_blurY, Vector2.Zero, Color.White);

        spriteBatch.End();
    }
}