using System.IO;
using System.Text.Json;
using LeagueHistory.Core.Architecture.Interfaces;

namespace LeagueHistory.Core.Architecture.Implementations
{
   

    public class SettingsProvider : ISettingsProvider
    {
        public string Location { get; }

        public SettingsProvider(string location = "settings.json")
        {
            Location = location;
        }

        public Settings? ReadSettings() => 
            !File.Exists(Location) ? new Settings() : JsonSerializer.Deserialize<Settings>(File.ReadAllText(Location));

        public void WriteSettings(Settings settings)
        {
            File.WriteAllText(Location,JsonSerializer.Serialize(settings));
        }
    }
}