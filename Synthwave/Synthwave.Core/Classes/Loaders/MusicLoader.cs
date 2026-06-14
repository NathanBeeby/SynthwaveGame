namespace Synthwave.Core.Classes.Loaders;

public class Music { public string Path; }
public static class MusicLoader
{
    public static Music Load(string path)
    {
        return new Music { Path = path };
    }
}