using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.World.Roads;

public struct RoadVertex(Vector3 pos, Vector3 normal, Vector2 uv) : IVertexType
{
    public Vector3 Position = pos;
    public Vector3 Normal = normal;
    public Vector2 UV = uv;
    // UV.X = across road (left->right)
    // UV.Y = along road

    public static VertexDeclaration VertexDeclaration = new(
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
    );

    readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}
