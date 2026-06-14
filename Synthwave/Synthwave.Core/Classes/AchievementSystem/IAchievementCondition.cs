namespace Synthwave.Core.Classes.AchievementSystem;

public interface IAchievementCondition
{
    void Initialize(AchievementContext context);
    void Evaluate(AchievementContext context);
    bool IsCompleted { get; }

    float Progress { get; }
}
