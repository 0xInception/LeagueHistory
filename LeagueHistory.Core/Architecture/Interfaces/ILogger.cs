namespace LeagueHistory.Core.Architecture.Interfaces
{
    public interface ILogger
    {
        void Success(string message);
        void Warning(string message);
        void Error(string message);
        void Debug(string message);
        void Info(string message);
    }
}