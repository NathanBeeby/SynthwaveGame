using Synthwave.Core.Classes.Core.Enums;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.AchievementSystem;

public class Achievement
{
    public string Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string IconKey { get; set; }

    public AchievementType Type { get; set; }

    public DateTime? UnlockedDate { get; set; }

    // Progress system (0–1)
    public float Progress { get; set; }

    public bool IsUnlocked => UnlockedDate.HasValue;

    public List<IAchievementCondition> Conditions { get; set; } = new();
}