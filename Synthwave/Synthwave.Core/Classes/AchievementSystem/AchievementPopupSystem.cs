using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Synthwave.Core.Classes.AchievementSystem;

public class AchievementPopupSystem
{
    #region Properties
    public float SlideX;
    public float Alpha;
    public float Scale;

    private float _timer;
    private bool _active;

    public event Action OnPlaySound;
    public event Action<Vector3> OnSpawnParticles;
    #endregion

    #region Methods
    public void Show(Achievement achievement, Texture2D icon)
    {
        SlideX = 500f; // off-screen
        Alpha = 0f;
        Scale = 0.8f;

        _timer = 0f;
        _active = true;

        OnPlaySound?.Invoke();
        OnSpawnParticles?.Invoke(new Vector3(0, 1, 0));
    }

    public void Update(float dt)
    {
        if (!_active) return;

        _timer += dt;
        SlideX = MathHelper.Lerp(SlideX, 0f, dt * 8f); // slide-in
        Alpha = MathHelper.Lerp(Alpha, 1f, dt * 8f); // fade-in
        Scale = 1f + (float)Math.Sin(_timer * 10f) * 0.05f;  // glow pulse

        if (_timer > 3f) _active = false;
    }
    #endregion
}