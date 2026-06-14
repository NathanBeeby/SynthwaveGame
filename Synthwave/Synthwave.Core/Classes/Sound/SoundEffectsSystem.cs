using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Sound;

public class SoundEffectsSystem
{
    private readonly Dictionary<string, string> _sounds = [];
    private readonly AudioPool _pool = new();

    public float Volume { get; set; } = 1f;
    private bool _muted;

    public void Register(string name, string path)
    {
        _sounds[name] = path;
    }

    public void Play(string name, Vector3? position = null)
    {
        if (_muted) return;
        if (!_sounds.TryGetValue(name, out var path)) return;

        var instance = _pool.Get(name);

        if (instance == null)
        {
            instance = AudioBackend.PlaySfx(path, Volume, position);
        }
        else
        {
            AudioBackend.SetSfxVolume(instance, Volume);
        }

        // Fire-and-forget style (you can track if needed)
    }

    public void SetMute(bool mute)
    {
        _muted = mute;
    }
}