using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Math;
using Synthwave.Core.Classes.World;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Roads;

public class RoadMeshRenderer
{
    private readonly GraphicsDevice device;
    private readonly BasicEffect effect;
    private Camera3D camera;

    public RoadMeshRenderer(GraphicsDevice graphicsDevice)
    {
        device = graphicsDevice;

        effect = new BasicEffect(device)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };
    }

    public void SetCamera(Camera3D cam) => camera = cam;

    public void DrawRoadMesh(Spline road, TerrainSystem terrain)
    {
        if (camera == null) return;

        List<VertexPositionColor> verts = [];

        float width = 6f;

        Vector3 prevLeft = Vector3.Zero;
        Vector3 prevRight = Vector3.Zero;

        bool first = true;

        for (float t = 0; t <= 1f; t += 0.03f)
        {
            Vector3 p = road.Evaluate(t);
            p.Y = terrain.GetHeight(p.X, p.Z);

            Vector3 forward = Tangent(road, t);
            forward = Vector3.Normalize(new Vector3(forward.X, 0, forward.Z));

            Vector3 right = Vector3.Cross(Vector3.Up, forward);

            Vector3 left = p - right * width;
            Vector3 rightPt = p + right * width;

            left.Y = terrain.GetHeight(left.X, left.Z);
            rightPt.Y = terrain.GetHeight(rightPt.X, rightPt.Z);

            if (!first)
            {
                verts.Add(new VertexPositionColor(prevLeft, Color.HotPink));
                verts.Add(new VertexPositionColor(prevRight, Color.HotPink));
                verts.Add(new VertexPositionColor(left, Color.HotPink));

                verts.Add(new VertexPositionColor(left, Color.HotPink));
                verts.Add(new VertexPositionColor(prevRight, Color.HotPink));
                verts.Add(new VertexPositionColor(rightPt, Color.HotPink));
            }

            prevLeft = left;
            prevRight = rightPt;
            first = false;
        }

        DrawTriangles(verts);
    }

    private static Vector3 Tangent(Spline road, float t)
    {
        float eps = 0.02f;

        return road.Evaluate(MathHelper.Clamp(t + eps, 0, 1)) -
               road.Evaluate(MathHelper.Clamp(t - eps, 0, 1));
    }

    private void DrawTriangles(List<VertexPositionColor> verts)
    {
        effect.World = Matrix.Identity;
        effect.View = camera.View;
        effect.Projection = camera.Projection;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            device.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                verts.ToArray(),
                0,
                verts.Count / 3
            );
        }
    }
}