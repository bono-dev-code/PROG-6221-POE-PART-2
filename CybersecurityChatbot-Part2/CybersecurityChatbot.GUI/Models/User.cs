using System;
using System.Collections.Generic;

namespace CybersecurityChatbot.Models
{
    public class User
    {
        public string Name { get; set; }
        public DateTime SessionStart { get; set; }
        public int MessagesExchanged { get; set; }

        public string FavoriteTopic { get; set; } = string.Empty;
        public string LastTopic { get; set; } = string.Empty;
        public string CurrentSentiment { get; set; } = "neutral";
        public string LastQuestion { get; set; } = string.Empty;

        public Dictionary<string, string> MemoryFacts { get; set; } = new();

        public User()
        {
            Name = "Guest";
            SessionStart = DateTime.Now;
            MessagesExchanged = 0;
        }

        public User(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Guest" : name;
            SessionStart = DateTime.Now;
            MessagesExchanged = 0;
        }

        public void IncrementMessageCount()
        {
            MessagesExchanged++;
        }
    }
}