using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Roads;

public class RoadManager
{
    #region Properties
    private readonly GraphicsDevice graphics;
    private VertexPositionColor[] roadVertices;
    private VertexPositionColor[] centerLineVertices;
    private readonly float roadWidth = 20f;
    private readonly SplineRoad road;
    #endregion

    #region Constructor
    public RoadManager(GraphicsDevice graphics)
    {
        this.graphics = graphics;
        road = new SplineRoad();
        Build();
    }
    #endregion

    #region Methods
    private void Build()
    {
        BuildRoadEdges();
        BuildCenterLine();
    }

    private void BuildRoadEdges()
    {
        List<VertexPositionColor> verts = [];

        foreach (var point in road.Points)
        {
            verts.Add(new VertexPositionColor(new Vector3(-roadWidth, 0.2f, point.Z), Color.Cyan));
            verts.Add(new VertexPositionColor(new Vector3(-roadWidth, 0.2f, point.Z + 20f), Color.Cyan));
            verts.Add(new VertexPositionColor(new Vector3(roadWidth, 0.2f, point.Z), Color.Cyan));
            verts.Add(new VertexPositionColor(new Vector3(roadWidth, 0.2f, point.Z + 20f), Color.Cyan));
        }

        roadVertices = [.. verts];
    }

    private void BuildCenterLine()
    {
        List<VertexPositionColor> verts = [];

        foreach (var point in road.Points)
        {
            verts.Add(new VertexPositionColor(point + new Vector3(0, 0.3f, 0), Color.Yellow));
            verts.Add(new VertexPositionColor(point + new Vector3(0, 0.3f, 10), Color.Yellow));
        }

        centerLineVertices = [.. verts];
    }

    public void Draw(GraphicsDevice graphics)
    {
        if (roadVertices != null) graphics.DrawUserPrimitives(PrimitiveType.LineList, roadVertices, 0, roadVertices.Length / 2);
        if (centerLineVertices != null) graphics.DrawUserPrimitives(PrimitiveType.LineList, centerLineVertices, 0, centerLineVertices.Length / 2);
    }
    #endregion
}