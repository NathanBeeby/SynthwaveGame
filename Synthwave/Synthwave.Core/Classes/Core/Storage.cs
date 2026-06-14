using Synthwave.Core.Classes.Core.Interfaces;
using System;
using System.IO;
using System.Text.Json;

namespace Synthwave.Core.Classes.Core;


public class Storage : IStorage
{
    #region Properties
    private const string FileName = "achievements.json";
    private readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };
    #endregion

    #region Methods
    public string GetPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),FileName);
    
    public void Save(SavedData sData)
    {
        var json = JsonSerializer.Serialize(sData, SerializerOptions);

        File.WriteAllText(GetPath(), json);
    }

    public SavedData Load()
    {
        var path = GetPath();

        if (!File.Exists(path)) return new SavedData();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<SavedData>(json) ?? new SavedData();
    }
    #endregion
}
