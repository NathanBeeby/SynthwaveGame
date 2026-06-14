using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Particles;

public class ParticleType
{
    public string Name;

    public Texture2D Texture;

    public float MinLifeTime;
    public float MaxLifeTime;

    public float MinSpeed;
    public float MaxSpeed;

    public float MinSize;
    public float MaxSize;

    public Color StartColor;
    public Color EndColor;

    public float EmissionRate;
}