using System;

namespace Synthwave.Core.Classes.Sound;

public static class AudioEvents
{
    public static event Action<string> OnSfxRequested;
    public static event Action<string> OnMusicRequested;
    public static event Action<string> OnMusicNext;
    public static event Action<string> OnMusicStop;

    public static void RaiseSfx(string name) => OnSfxRequested?.Invoke(name);
    public static void RaiseMusic(string name) => OnMusicRequested?.Invoke(name);
}
