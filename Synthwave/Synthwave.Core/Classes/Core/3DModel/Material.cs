using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Core;

public class Material
{
    public Effect Effect;

    public Texture2D Diffuse;
    public Texture2D Emissive;

    public Color BaseColor = Color.White;

    public float EmissiveStrength = 0f;
    public bool UseEmissive = false;
}
