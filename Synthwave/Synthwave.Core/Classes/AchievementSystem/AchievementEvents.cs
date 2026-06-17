using System;

namespace Synthwave.Core.Classes.AchievementSystem;

public static class AchievementEvents
{
    public static event Action<string> OnAchievementTriggered;
    public static void Trigger(string achievementId) => OnAchievementTriggered?.Invoke(achievementId);
}
