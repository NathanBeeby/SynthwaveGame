namespace Synthwave.Core.Classes.Loaders;

public class Asset<T>(string key, T value)
{
    public string Key { get; } = key;
    public T Value { get; private set; } = value;

    public int RefCount { get; private set; } = 1;

    public void AddRef() => RefCount++;
    public void Release() => RefCount--;
}