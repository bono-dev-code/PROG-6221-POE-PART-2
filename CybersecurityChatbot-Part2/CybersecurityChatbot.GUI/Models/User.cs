using System;
using System.Collections.Generic;

namespace CybersecurityChatbot.Models
{
    // This class stores information about the current chatbot user.
    // It helps the chatbot remember the user's name, session details, topic, mood, and memory facts.
    public class User
    {
        // Stores the user's name
        public string Name { get; set; }

        // Stores the date and time when the chat session started
        public DateTime SessionStart { get; set; }

        // Stores how many messages the user has sent or exchanged with the chatbot
        public int MessagesExchanged { get; set; }

        // Stores the cybersecurity topic the user likes most
        public string FavoriteTopic { get; set; } = string.Empty;

        // Stores the last topic the user asked about
        public string LastTopic { get; set; } = string.Empty;

        // Stores the current mood or feeling detected from the user
        public string CurrentSentiment { get; set; } = "neutral";

        // Stores the last question asked by the user
        public string LastQuestion { get; set; } = string.Empty;

        // Stores extra facts the chatbot remembers about the user
        public Dictionary<string, string> MemoryFacts { get; set; } = new();

        // Default constructor used when no user name is provided
        public User()
        {
            // Set the default name as Guest
            Name = "Guest";

            // Record the current date and time as the start of the session
            SessionStart = DateTime.Now;

            // Start the message count at zero
            MessagesExchanged = 0;
        }

        // Constructor used when the user provides a name
        public User(string name)
        {
            // If the name is empty, use Guest instead
            Name = string.IsNullOrWhiteSpace(name) ? "Guest" : name;

            // Record the current date and time as the start of the session
            SessionStart = DateTime.Now;

            // Start the message count at zero
            MessagesExchanged = 0;
        }

        // This method increases the message count by one
        public void IncrementMessageCount()
        {
            MessagesExchanged++;
        }
    }
}