using System;
using System.IO;
using System.Text.Json;

namespace LeagueHistory.Core.Architecture
{
    public class Settings
    {
        public string[] Accounts { get; set; } = Array.Empty<string>();
    }
}