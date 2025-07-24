using System;
using System.IO;
using System.Text.Json;
using PirateAdventures.Input;

namespace PirateAdventures.Settings;

public static class SettingsManager
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "input.json");  // ./bin/Debug/net6.0-windows/input.json
    
    public static void SaveSettings(InputSettings settings)
    {
        Console.WriteLine("Writing settings to file "+FilePath+"...");
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public static InputSettings LoadSettings()
    {
        Console.WriteLine("Reading settings from file...");
        
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<InputSettings>(json);
        }

        return new InputSettings();
    }
}