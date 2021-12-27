namespace LeagueHistory.Core.Architecture.Interfaces
{
    public interface ISettingsProvider
    {
        Settings? ReadSettings();
        void WriteSettings(Settings settings);
    }
}