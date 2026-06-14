using Synthwave.Core.Classes.Core.Interfaces;
using System;

namespace Synthwave.Core.Classes.AchievementSystem;

public class SteamAchievements : IPlatformAchievements
{
    public void Unlock(string id)
    {
        // Steam API call placeholder
        Console.WriteLine($"Steam unlock: {id}");
    }

    public void SetProgress(string id, float progress)
    {
        Console.WriteLine($"Steam progress {id}: {progress:P0}");
    }
}
