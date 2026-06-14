using System;

namespace Synthwave.Core.Classes.AchievementSystem;

public class AchievementNotifier
{
    #region Events
    public event Action<Achievement> OnShowPopup;
    #endregion

    #region Constructor
    public AchievementNotifier(AchievementSystem system) => system.OnUnlocked += Show;
    #endregion

    #region Methods
    private void Show(Achievement achievement) => OnShowPopup?.Invoke(achievement);
    #endregion
}