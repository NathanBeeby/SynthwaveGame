using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Particles;

public class TrailRenderer
{
    private List<TrailSegment> _segments = new();

    public float MaxLife = 1f;

    public void Add(Vector3 position)
    {
        _segments.Add(new TrailSegment
        {
            Position = position,
            Age = 0f
        });
    }

    public void Update(float dt)
    {
        for (int i = _segments.Count - 1; i >= 0; i--)
        {
            _segments[i].Age += dt;

            if (_segments[i].Age > MaxLife)
                _segments.RemoveAt(i);
        }
    }
}