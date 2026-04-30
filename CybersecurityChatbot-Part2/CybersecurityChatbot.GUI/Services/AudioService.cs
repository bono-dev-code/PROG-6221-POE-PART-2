using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot.Services
{
    // This class is responsible for playing audio in the chatbot.
    // It is mainly used to play the greeting sound when the app starts.
    public class AudioService
    {
        // Stores the path (location) of the audio file
        private readonly string _audioFilePath;

        // Constructor that receives the audio file path
        public AudioService(string audioFilePath)
        {
            // Save the audio file path so it can be used later
            _audioFilePath = audioFilePath;
        }

        // This method plays the greeting audio
        public void PlayGreeting()
        {
            try
            {
                // Check if the file path is not empty AND the file actually exists
                if (!string.IsNullOrWhiteSpace(_audioFilePath) && File.Exists(_audioFilePath))
                {
                    // Create a SoundPlayer object using the file path
                    using SoundPlayer player = new SoundPlayer(_audioFilePath);

                    // Load the audio file into memory
                    player.Load();

                    // Play the audio and wait until it finishes (synchronous play)
                    player.PlaySync();
                }
            }
            catch
            {
                // If anything goes wrong, do nothing.
                // This ensures the chatbot does NOT crash because of audio issues.
            }
        }
    }
}