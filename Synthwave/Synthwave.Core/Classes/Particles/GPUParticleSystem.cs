using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthwave.Core.Classes.Particles;

public class GPUParticleSystem
{
    private DynamicVertexBuffer _vertexBuffer;
    private GraphicsDevice _graphics;

    private ParticleVertex[] _cpuBuffer;

    public GPUParticleSystem(GraphicsDevice graphics, int maxParticles)
    {
        _graphics = graphics;
        _cpuBuffer = new ParticleVertex[maxParticles];

        _vertexBuffer = new DynamicVertexBuffer(
            graphics,
            ParticleVertex.VertDeclaration,
            maxParticles,
            BufferUsage.WriteOnly
        );
    }

    public void Upload(List<Particle> particles)
    {
        int count = Math.Min(particles.Count, _cpuBuffer.Length);

        for (int i = 0; i < count; i++)
        {
            var p = particles[i];

            _cpuBuffer[i] = new ParticleVertex
            {
                Position = p.Position,
                Color = p.Color,
                Size = p.Size,
                UV = Vector2.Zero
            };
        }

        _vertexBuffer.SetData(_cpuBuffer, 0, count);
    }

    public void Draw(Texture2D texture, Camera3D camera)
    {
        _graphics.SetVertexBuffer(_vertexBuffer);

        // shader-based rendering (next section)
    }
}
