namespace Synthwave.Core.Classes.Core.Interfaces;

public interface IStorage
{
    string GetPath();
    void Save(SavedData sData);
    SavedData Load();
}
