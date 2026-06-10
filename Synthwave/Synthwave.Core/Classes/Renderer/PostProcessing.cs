using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Renderer;

public class PostProcessing
{
    public float BloomIntensity = 1.6f;

    private RenderTarget2D sceneTarget;

    public void Initialize(GraphicsDevice device)
    {
        sceneTarget = new RenderTarget2D(
            device,
            device.Viewport.Width,
            device.Viewport.Height
        );
    }

    public RenderTarget2D Begin(GraphicsDevice device)
    {
        device.SetRenderTarget(sceneTarget);
        return sceneTarget;
    }

    public void End(GraphicsDevice device)
    {
        device.SetRenderTarget(null);
    }

    public void Apply(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

        // bloom pass placeholder (you can later plug HLSL here)
        spriteBatch.Draw(sceneTarget, Vector2.Zero, Color.White * BloomIntensity);

        spriteBatch.End();
    }
}