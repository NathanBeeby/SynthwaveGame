namespace Synthwave.Core.Classes.Loaders;

public class Shader { public string Path; }

public static class ShaderLoader
{
    public static Shader Load(string path)
    {
        return new Shader { Path = path };
    }
}
