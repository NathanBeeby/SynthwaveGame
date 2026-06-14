using System;

namespace Synthwave.Core.Classes.Sound;

public class MusicSystem
{
    private object _current;
    private object _next;

    private string _currentPath;

    public float Volume { get; set; } = 1f;

    private readonly float _crossfadeTime = 2f;
    private float _timer;
    private bool _isCrossfading;

    public void Play(string path)
    {
        Stop();

        _currentPath = path;
        _current = AudioBackend.PlayMusic(path, Volume, loop: true);
    }

    public void CrossfadeTo(string newTrack)
    {
        if (_currentPath == newTrack) return;

        _next = AudioBackend.PlayMusic(newTrack, 0f, loop: true);

        _timer = 0f;
        _isCrossfading = true;
    }

    public void Stop()
    {
        if (_current != null)
            AudioBackend.StopMusic(_current);

        if (_next != null)
            AudioBackend.StopMusic(_next);

        _current = null;
        _next = null;
        _isCrossfading = false;
    }

    public void Update(float deltaTime)
    {
        if (!_isCrossfading) return;

        _timer += deltaTime;

        float t = Math.Clamp(_timer / _crossfadeTime, 0f, 1f);

        AudioBackend.SetMusicVolume(_current, Volume * (1f - t));
        AudioBackend.SetMusicVolume(_next, Volume * t);

        if (t >= 1f)
        {
            AudioBackend.StopMusic(_current);
            _current = _next;
            _next = null;
            _isCrossfading = false;
        }
    }
}