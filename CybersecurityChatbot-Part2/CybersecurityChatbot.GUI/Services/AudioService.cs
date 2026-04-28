using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot.Services
{
    public class AudioService
    {
        private readonly string _audioFilePath;

        public AudioService(string audioFilePath)
        {
            _audioFilePath = audioFilePath;
        }

        public void PlayGreeting()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_audioFilePath) && File.Exists(_audioFilePath))
                {
                    using SoundPlayer player = new SoundPlayer(_audioFilePath);
                    player.Load();
                    player.PlaySync();
                }
            }
            catch
            {
                // Audio should never crash the chatbot.
            }
        }
    }
}
