using PirateAdventures.Input;
using System;
using System.IO;
using System.Text.Json;

namespace PirateAdventures.Settings;

public static class SettingsManager
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "input.json");  // ./bin/Debug/net8.0-windows/input.json
    public static float Volume { get; set; } = 0.1f;

    public static void SaveSettings(InputSettings settings)
    {
        Console.WriteLine("Writing settings to file " + FilePath + "...");
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public static InputSettings LoadSettings()
    {
        Console.WriteLine("Reading settings from file...");

        if (File.Exists(FilePath))
        {
            var defaults = new InputSettings();
            string json = File.ReadAllText(FilePath);
            var loaded = JsonSerializer.Deserialize<InputSettings>(json);

            // make sure no keybindings are missing in the json
            foreach (var kvp in defaults.KeyBindings)
            {
                loaded.KeyBindings ??= [];

                if (!loaded.KeyBindings.ContainsKey(kvp.Key))
                    loaded.KeyBindings[kvp.Key] = kvp.Value;
            }
        }

        return new InputSettings();
    }
}