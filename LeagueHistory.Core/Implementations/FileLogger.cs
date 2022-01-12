using System;
using System.IO;
using LeagueHistory.Core.Interfaces;

namespace LeagueHistory.Core.Implementations
{
    public class FileLogger : ILogger
    {
        public string Path { get; }

        public FileLogger(string path = "debug.txt")
        {
            Path = path;
        }
        public void Success(string message) => WriteToFile('+', message);

        public void Warning(string message) => WriteToFile('!', message);

        public void Error(string message) => WriteToFile('-', message);

        public void Debug(string message) => WriteToFile('@', message);

        public void Info(string message) => WriteToFile('$', message);
        private void WriteToFile(char c,string message)
        {
            File.AppendAllText(Path,message+Environment.NewLine);
        }
    }
}