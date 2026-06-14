namespace Synthwave.Core.Classes.AchievementSystem;

public class AchievementDataStore(AchievementSystem _sys)
{
    #region Properties
    private readonly AchievementSystem _achievementSystem = _sys;
    #endregion

    #region Methods
    public void RegisterAchievements()
    {
        // Add Achievements here to register the achievements
        _achievementSystem.Manager.Register(ReturnAchievement("FirstGame", "The First Ever!", "Play the game Once", "First_Game_Icon"));
    }

    private static Achievement ReturnAchievement(string ID, string Title, string Description, string IconKey)
    {
        return new Achievement
        {
            Id = ID,
            Title = Title,
            Description = Description,
            IconKey = IconKey
        };
    }
    #endregion
}
