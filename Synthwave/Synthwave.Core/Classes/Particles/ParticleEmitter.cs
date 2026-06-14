using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Particles;

public class ParticleEmitter(ParticleManager manager, ParticleType type)
{
    private readonly ParticleManager _manager = manager;
    private readonly ParticleType _type = type;

    public Vector3 Position;

    private float _timer;

    public bool Looping = true;

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _timer += dt;

        float emitInterval = 1f / _type.EmissionRate;

        while (_timer >= emitInterval)
        {
            _timer -= emitInterval;

            EmitParticle();
        }
    }

    private void EmitParticle()
    {
        var random = _manager.Random;

        var particle = new Particle
        {
            Position = Position,

            Velocity = new Vector3(
                (float)(random.NextDouble() - 0.5f),
                (float)(random.NextDouble()),
                (float)(random.NextDouble() - 0.5f)
            ) * MathHelper.Lerp(_type.MinSpeed, _type.MaxSpeed, (float)random.NextDouble()),

            LifeTime = MathHelper.Lerp(_type.MinLifeTime, _type.MaxLifeTime, (float)random.NextDouble()),

            Size = MathHelper.Lerp(_type.MinSize, _type.MaxSize, (float)random.NextDouble()),

            Color = _type.StartColor,

            Age = 0f
        };

        _manager.Spawn(particle);
    }
}