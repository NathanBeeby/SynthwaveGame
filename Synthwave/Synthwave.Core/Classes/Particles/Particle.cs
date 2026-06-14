using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Particles;

public class Particle
{
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Acceleration;

    public float LifeTime;
    public float Age;

    public float Size;
    public float Rotation;

    public Color Color;
    public bool EmitsLight;
    public float LightIntensity;

    public bool IsAlive => Age < LifeTime;
}