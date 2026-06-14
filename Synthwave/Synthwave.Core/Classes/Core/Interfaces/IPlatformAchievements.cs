namespace Synthwave.Core.Classes.Core.Interfaces;

public interface IPlatformAchievements
{
    void Unlock(string id);
    void SetProgress(string id, float progress);
}
