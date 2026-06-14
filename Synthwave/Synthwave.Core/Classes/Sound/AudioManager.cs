namespace Synthwave.Core.Classes.Sound;

public class AudioManager
{
    public MusicSystem Music { get; } = new();
    public SoundEffectsSystem SFX { get; } = new();
    public AudioPool Pool { get; } = new();

    public AudioManager()
    {
        // Event-driven binding
        AudioEvents.OnSfxRequested += PlaySfx;
        AudioEvents.OnMusicRequested += Music.CrossfadeTo;
    }

    private void PlaySfx(string name) => SFX.Play(name);
    public void Update(float deltaTime) => Music.Update(deltaTime);
}

/*
 var audio = new AudioManager();

// Music
audio.Music.AddTrack(new MusicTrack("Neon City", "music/neon1.mp3"));
audio.Music.AddTrack(new MusicTrack("Highway", "music/neon2.mp3"));
audio.Music.SetLoopPlaylist(true);
audio.Music.Play();

// SFX
audio.SFX.Register(new SoundEffect("collect", "sfx/collect.wav"));
audio.SFX.Register(new SoundEffect("hit", "sfx/hit.wav"));
 
crossfade
audio.Music.CrossfadeTo("music/highway.mp3");

or event-driven
audio.Music.CrossfadeTo("music/highway.mp3");

SFX with 3D position
audio.SFX.Play("explosion", enemy.Position);

Collectable
AudioEvents.RaiseSfx("collect");

Hit
AudioEvents.RaiseSfx("hit");

 */