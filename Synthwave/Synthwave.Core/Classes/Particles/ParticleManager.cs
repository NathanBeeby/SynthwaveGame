using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Particles;

public class ParticleManager
{
    private readonly List<Particle> _particles = [];
    private readonly List<Particle> _pool = [];

    public readonly Random Random = new();
    public List<ParticleField> Fields = [];

    private void ApplyFields(Particle p, float dt)
    {
        foreach (var field in Fields)
        {
            p.Velocity = field.Apply(p.Position, p.Velocity);
        }
    }
    public void Spawn(Particle particle)
    {
        _particles.Add(particle);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];

            p.Age += dt;

            if (!p.IsAlive)
            {
                _particles.RemoveAt(i);
                _pool.Add(p);
                continue;
            }

            p.Velocity += p.Acceleration * dt;
            p.Position += p.Velocity * dt;

            // fade over lifetime
            float t = p.Age / p.LifeTime;
            p.Color = Color.Lerp(Color.White, Color.Transparent, t);
        }
    }

    public void Draw(GraphicsDevice graphics, Camera3D camera, Texture2D texture)
    {
        var spriteBatch = new SpriteBatch(graphics);

        spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.Additive,SamplerState.LinearClamp,DepthStencilState.DepthRead,RasterizerState.CullNone);

        foreach (var p in _particles)
        {
            Vector2 screenPos = ProjectToScreen(p.Position, camera, graphics);
            float size = p.Size * (1f - (p.Age / p.LifeTime));
            spriteBatch.Draw(texture,screenPos,null,p.Color,0f,Vector2.Zero,size,SpriteEffects.None,0f);
        }

        spriteBatch.End();
    }

    private Vector2 ProjectToScreen(Vector3 world, Camera3D camera, GraphicsDevice graphics)
    {
        var viewport = graphics.Viewport;
        var projected = viewport.Project(world,camera.Projection,camera.View,Matrix.Identity);
        return new Vector2(projected.X, projected.Y);
    }
}

/*
 Examples:
var explosion = new ParticleType
{
    Name = "Explosion",
    EmissionRate = 200,
    MinLifeTime = 0.3f,
    MaxLifeTime = 1f,
    MinSpeed = 2f,
    MaxSpeed = 10f,
    MinSize = 0.1f,
    MaxSize = 0.4f,
    StartColor = Color.OrangeRed,
    EndColor = Color.Transparent
};

Neon Glow:
var glow = new ParticleType
{
    Name = "Glow",
    EmissionRate = 10,
    MinLifeTime = 1f,
    MaxLifeTime = 2f,
    MinSpeed = 0.1f,
    MaxSpeed = 0.5f,
    MinSize = 0.2f,
    MaxSize = 0.5f,
    StartColor = Color.Cyan,
    EndColor = Color.Transparent
};
 

Exhaust Smoke:
var smoke = new ParticleType
{
    Name = "Smoke",
    EmissionRate = 30,
    MinLifeTime = 1f,
    MaxLifeTime = 3f,
    MinSpeed = 0.2f,
    MaxSpeed = 1f,
    MinSize = 0.2f,
    MaxSize = 0.6f,
    StartColor = Color.Gray,
    EndColor = Color.Transparent
};

Attach emitter to object:
var emitter = new ParticleEmitter(particleManager, explosion);
emitter.Position = enemy.Position;

Update loop:
particleManager.Update(gameTime);
emitter.Update(gameTime);

Trigger Effects:
emitter.Position = enemy.Position;
emitter.Update(gameTime);

 */