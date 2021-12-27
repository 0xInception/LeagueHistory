namespace LeagueHistory.Core.Interfaces
{
    public interface ISettingsProvider
    {
        Settings? ReadSettings();
        void WriteSettings(Settings settings);
    }
}