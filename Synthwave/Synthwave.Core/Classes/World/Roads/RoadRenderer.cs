using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Models;

namespace Synthwave.Core.Classes.World.Roads;

public class RoadRenderer(GraphicsDevice device, Effect effect)
{
    private readonly GraphicsDevice _device = device;
    private readonly Effect _effect = effect;
    private Camera3D _camera;

    public void SetCamera(Camera3D cam) => _camera = cam;

    public void Draw(WorldChunk chunk, float time)
    {
        if (_camera == null || chunk.RoadVB == null) return;

        _effect.Parameters["World"]?.SetValue(Matrix.Identity);
        _effect.Parameters["View"]?.SetValue(_camera.View);
        _effect.Parameters["Projection"]?.SetValue(_camera.Projection);
        _effect.Parameters["Time"]?.SetValue(time);
        _effect.Parameters["CameraPosition"]?.SetValue(_camera.Position);

        _device.SetVertexBuffer(chunk.RoadVB);
        _device.Indices = chunk.RoadIB;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.RoadIB.IndexCount / 3);
        }
    }
}