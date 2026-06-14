namespace Synthwave.Core.Classes.Loaders;

public class Model { public string Path; }
public static class ModelLoader
{
    public static Model Load(string path)
    {
        return new Model { Path = path };
    }
}