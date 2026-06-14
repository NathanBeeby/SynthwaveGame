using System;
using System.Collections.Generic;
using System.Linq;

namespace Synthwave.Core.Classes.AchievementSystem;

public class AchievementManager
{
    #region Properties
    private readonly Dictionary<string, Achievement> _achievements = new();

    public event Action<Achievement> OnUnlocked;
    public event Action<Achievement> OnProgressChanged;

    private AchievementContext _context;
    #endregion

    #region Methods
    public void SetContext(AchievementContext context)
    {
        _context = context;

        foreach (var ach in _achievements.Values)
        {
            foreach (var c in ach.Conditions)
                c.Initialize(context);
        }
    }

    public void Register(Achievement achievement) => _achievements[achievement.Id] = achievement;

    public void Unlock(string id)
    {
        if (!_achievements.TryGetValue(id, out var ach)) return;
        if (ach.IsUnlocked) return;

        ach.UnlockedDate = DateTime.UtcNow;
        ach.Progress = 1f;

        OnUnlocked?.Invoke(ach);
    }

    public void Update()
    {
        foreach (var ach in _achievements.Values)
        {
            if (ach.IsUnlocked) continue;

            float totalProgress = 0f;

            if (ach.Conditions.Count > 0)
            {
                foreach (var c in ach.Conditions)
                {
                    c.Evaluate(_context);
                    totalProgress += c.Progress;
                }

                ach.Progress = totalProgress / ach.Conditions.Count;
            }

            OnProgressChanged?.Invoke(ach);

            if (ach.Conditions.All(c => c.IsCompleted)) Unlock(ach.Id);
        }
    }

    public IEnumerable<Achievement> GetAll() => _achievements.Values;
    #endregion
}