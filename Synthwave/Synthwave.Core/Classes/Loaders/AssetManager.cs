using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Loaders;

public class AssetManager
{
    private readonly Dictionary<string, object> _assets = [];

    #region Generic Load/Get

    public T Get<T>(string key, Func<T> loader)
    {
        if (_assets.TryGetValue(key, out var existing))
        {
            var asset = (Asset<T>)existing;
            asset.AddRef();
            return asset.Value;
        }

        T value = loader();
        _assets[key] = new Asset<T>(key, value);

        return value;
    }

    #endregion

    #region Release
    public void Release<T>(string key)
    {
        if (!_assets.TryGetValue(key, out var existing)) return;

        var asset = (Asset<T>)existing;
        asset.Release();

        if (asset.RefCount <= 0)
        {
            DisposeIfNeeded(asset.Value);
            _assets.Remove(key);
        }
    }

    private static void DisposeIfNeeded<T>(T asset)
    {
        // Optional engine-specific cleanup
        if (asset is IDisposable disposable) disposable.Dispose();
    }

    #endregion

    #region Bulk cleanup

    public void Clear()
    {
        foreach (var kv in _assets)
        {
            if (kv.Value is Asset<object> asset)
            {
                if (asset.Value is IDisposable d) d.Dispose();
            }
        }

        _assets.Clear();
    }

    #endregion
}

/*
Textures:
 Texture2D roadTex = assets.Get(
    "road",
    () => TextureLoader.Load("textures/road.png")
);
 
Music:
Music track = assets.Get(
    "neon_music",
    () => MusicLoader.Load("audio/neon1.mp3")
);
 

SFX:
Sfx collect = assets.Get(
    "collect_sfx",
    () => SfxLoader.Load("audio/collect.wav")
);

Models:
Model car = assets.Get(
    "car_model",
    () => ModelLoader.Load("models/car.fbx")
);

Unloading:
assets.Release<Texture2D>("road");
assets.Release<Sfx>("collect_sfx");

 */