using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Particles;

public struct ParticleVertex : IVertexType
{
    public Vector3 Position;
    public Vector2 UV;
    public Color Color;
    public float Size;

    public readonly VertexDeclaration VertexDeclaration => new(
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0),
        new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
    );

    public static VertexDeclaration VertDeclaration => new(
    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
    new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
    new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0),
    new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
);
}
