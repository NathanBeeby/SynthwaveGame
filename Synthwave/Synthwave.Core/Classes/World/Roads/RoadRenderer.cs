using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Models;

namespace Synthwave.Core.Classes.World.Roads;

public class RoadRenderer(GraphicsDevice device, Effect roadEffect, Effect sidewalkEffect, Effect centreLineEffect)
{
    private readonly GraphicsDevice _device = device;
    private readonly Effect _roadEffect = roadEffect;
    private readonly Effect _sidewalkEffect = sidewalkEffect;
    private readonly Effect _centreLineEffect = centreLineEffect;
    private Camera3D _camera;
    private Texture2D _reflectionTex;
    private float _reflectionStrength;

    private static readonly RasterizerState _roadRaster = new()
    {
        CullMode = CullMode.None
    };
    private static readonly RasterizerState _sidewalkRaster = new()
    {
        CullMode = CullMode.None,
        DepthBias = -0.00005f,
        SlopeScaleDepthBias = -1f
    };
    private static readonly RasterizerState _centreLineRaster = new()
    {
        CullMode = CullMode.None,
        DepthBias = -0.0001f,
        SlopeScaleDepthBias = -2f
    };

    public void SetCamera(Camera3D cam) => _camera = cam;

    public void SetReflectionSource(Texture2D tex, float strength)
    {
        _reflectionTex = tex;
        _reflectionStrength = strength;
    }

    public void Draw(WorldChunk chunk, float time, float wetness)
    {
        if (_camera == null) return;

        var prevRaster = _device.RasterizerState;

        _device.RasterizerState = _roadRaster;
        DrawRoadSurface(chunk, time, wetness);

        _device.RasterizerState = _sidewalkRaster;
        DrawSidewalk(chunk, time);

        _device.RasterizerState = _centreLineRaster;
        DrawCentreLine(chunk, time);

        _device.RasterizerState = prevRaster;
    }

    private void DrawRoadSurface(WorldChunk chunk, float time, float wetness)
    {
        if (chunk.RoadVB == null || chunk.RoadIB == null) return;

        float gloss = MathHelper.Lerp(0.65f, 1f, MathHelper.Clamp(wetness, 0f, 1f));

        _roadEffect.Parameters["World"]?.SetValue(Matrix.Identity);
        _roadEffect.Parameters["View"]?.SetValue(_camera.View);
        _roadEffect.Parameters["Projection"]?.SetValue(_camera.Projection);
        _roadEffect.Parameters["Time"]?.SetValue(time);
        _roadEffect.Parameters["CameraPosition"]?.SetValue(_camera.Position);
        _roadEffect.Parameters["Wetness"]?.SetValue(gloss);
        _roadEffect.Parameters["ReflectionTex"]?.SetValue(_reflectionTex);
        _roadEffect.Parameters["ReflectionStrength"]?.SetValue(_reflectionTex != null ? _reflectionStrength : 0f);

        _device.SetVertexBuffer(chunk.RoadVB);
        _device.Indices = chunk.RoadIB;
        foreach (var pass in _roadEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.RoadIB.IndexCount / 3);
        }
    }

    private void DrawSidewalk(WorldChunk chunk, float time)
    {
        if (chunk.SidewalkVB == null || chunk.SidewalkIB == null)
            return;

        _sidewalkEffect.Parameters["World"]?.SetValue(Matrix.Identity);
        _sidewalkEffect.Parameters["View"]?.SetValue(_camera.View);
        _sidewalkEffect.Parameters["Projection"]?.SetValue(_camera.Projection);
        _sidewalkEffect.Parameters["Time"]?.SetValue(time);

        _device.DepthStencilState = DepthStencilState.DepthRead;
        _device.BlendState = BlendState.Additive;

        _device.SetVertexBuffer(chunk.SidewalkVB);
        _device.Indices = chunk.SidewalkIB;

        foreach (var pass in _sidewalkEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0, 0,
                chunk.SidewalkIB.IndexCount / 3);
        }

        _device.BlendState = BlendState.Opaque;
        _device.DepthStencilState = DepthStencilState.Default;
    }

    private void DrawCentreLine(WorldChunk chunk, float time)
    {
        if (chunk.CentreLineVB == null || chunk.CentreLineIB == null)
            return;

        _centreLineEffect.Parameters["World"]?.SetValue(Matrix.Identity);
        _centreLineEffect.Parameters["View"]?.SetValue(_camera.View);
        _centreLineEffect.Parameters["Projection"]?.SetValue(_camera.Projection);
        _centreLineEffect.Parameters["Time"]?.SetValue(time);

        _device.DepthStencilState = DepthStencilState.DepthRead;
        _device.BlendState = BlendState.Additive;

        _device.SetVertexBuffer(chunk.CentreLineVB);
        _device.Indices = chunk.CentreLineIB;

        foreach (var pass in _centreLineEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0, 0,
                chunk.CentreLineIB.IndexCount / 3);
        }

        _device.BlendState = BlendState.Opaque;
        _device.DepthStencilState = DepthStencilState.Default;
    }
}