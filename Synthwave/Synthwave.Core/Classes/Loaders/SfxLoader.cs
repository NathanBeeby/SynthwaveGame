namespace Synthwave.Core.Classes.Loaders;

public class Sfx { public string Path; }
public static class SfxLoader
{
    public static Sfx Load(string path)
    {
        return new Sfx { Path = path };
    }
}