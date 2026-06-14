using Synthwave.Core.Classes.Core.Interfaces;
using System;

namespace Synthwave.Core.Classes.AchievementSystem;

public class AchievementSystem
{
    #region Properties
    public AchievementManager Manager { get; }

    private readonly IPlatformAchievements _platform;

    public event Action<Achievement> OnUnlocked;
    #endregion

    #region Constructor
    public AchievementSystem(IPlatformAchievements platform)
    {
        _platform = platform;
        Manager = new AchievementManager();

        Manager.OnUnlocked += HandleUnlock;
        Manager.OnProgressChanged += HandleProgress;
    }
    #endregion

    #region Methods
    private void HandleUnlock(Achievement a)
    {
        _platform.Unlock(a.Id);
        OnUnlocked?.Invoke(a);
    }

    private void HandleProgress(Achievement a) => _platform.SetProgress(a.Id, a.Progress);
    public void Update() => Manager.Update();
    #endregion
}

/*
 Trigger Example:
AchievementEvents.Trigger("first_collectible");

// UI System Listener
notifier.OnShowPopup += (achievement) =>
{
    ui.ShowAchievementPopup(
        achievement.Title,
        achievement.Description,
        assetManager.Get<Texture2D>(achievement.IconKey, ...)
    );
};

// Unlocking Achievement Event Hookup
achievementSystem.OnUnlocked += (a) =>
{
    var icon = assetManager.Get<Texture2D>(a.IconKey, ...);

    popupSystem.Show(a, icon);
};

// Playing Achievement sound hookup:
 popupSystem.OnPlaySound += () =>
{
    audio.SFX.Play("achievement_unlock");
};


// Spawning Particles event hookup:
 popupSystem.OnSpawnParticles += (pos) =>
{
    particleSystem.SpawnBurst(pos);
};
 
// Manual Trigger:
AchievementEvents.Trigger("win_race");

 */