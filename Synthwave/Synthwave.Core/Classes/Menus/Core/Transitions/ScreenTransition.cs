using Synthwave.Core.Classes.Core.Enums;

namespace Synthwave.Core.Classes.Menus.Core.Transitions;

public class ScreenTransition
{
    public bool IsFinished { get; protected set; }
    public float Alpha;
    public ScreenTransitionState State;

    public void FadeOut(float dt)
    {
        Alpha += dt * 2f;
        if (Alpha >= 1f) State = ScreenTransitionState.FadingIn;
    }

    public void FadeIn(float dt)
    {
        Alpha -= dt * 2f;
        if (Alpha <= 0f) State = ScreenTransitionState.None;
    }
}
/*
 To Use:
spriteBatch.Draw(TextureStore.Pixel, fullScreenRect, Color.Black * transition.Alpha);
 
 */
