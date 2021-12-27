using System;
using System.Linq;
using LeagueHistory.Core.Interfaces;

namespace LeagueHistory.Core.Implementations
{
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random;
        private const string ALPHANUMERIC = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";

        public RandomProvider()
        {
            _random = new Random();
        }

        public string RandomString(int length) =>
                new(Enumerable
                    .Repeat(ALPHANUMERIC, length)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}