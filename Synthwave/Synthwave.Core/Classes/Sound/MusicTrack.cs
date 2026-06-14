namespace Synthwave.Core.Classes.Sound;

public class MusicTrack(string name, string filePath)
{
    public string Name { get; } = name;
    public string FilePath { get; } = filePath;
}
