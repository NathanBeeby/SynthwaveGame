namespace Synthwave.Core.Classes.Sound;

public class SoundEffect(string name, string filePath)
{
    public string Name { get; } = name;
    public string FilePath { get; } = filePath;
}
