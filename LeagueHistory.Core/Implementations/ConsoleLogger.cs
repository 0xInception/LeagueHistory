using System;
using LeagueHistory.Core.Interfaces;

namespace LeagueHistory.Core.Implementations
{
    public class ConsoleLogger : ILogger
    {
        public void Success(string message) => Log('+', ConsoleColor.Cyan, message);

        public void Warning(string message) => Log('!', ConsoleColor.Yellow, message);

        public void Error(string message) => Log('-', ConsoleColor.Red, message);

        public void Debug(string message) => Log('@', ConsoleColor.Gray, message);

        public void Info(string message) => Log('$', ConsoleColor.Green, message);

        private void Log(char prefix, ConsoleColor color,string message)
        {
            Write("[",ConsoleColor.White);
            Write(prefix.ToString(),color);
            Write($"] {message + Environment.NewLine}",ConsoleColor.White);
        }

        private void Write(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ResetColor();
        }
    }
}