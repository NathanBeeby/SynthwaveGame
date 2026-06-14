using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Sound;

public static class AudioBackend
{
    // --- MUSIC ---
    public static object PlayMusic(string path, float volume, bool loop)
    {
        // Replace with real engine call
        return new object();
    }

    public static void StopMusic(object instance) { }
    public static void PauseMusic(object instance) { }
    public static void ResumeMusic(object instance) { }

    public static void SetMusicVolume(object instance, float volume) { }

    // --- SFX ---
    public static object PlaySfx(string path, float volume, Vector3? position = null)
    {
        // If position != null → 3D sound
        return new object();
    }

    public static void StopSfx(object instance) { }

    public static void SetSfxVolume(object instance, float volume) { }
}
