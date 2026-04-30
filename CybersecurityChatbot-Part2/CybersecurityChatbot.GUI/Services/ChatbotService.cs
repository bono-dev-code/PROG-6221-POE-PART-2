using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CybersecurityChatbot.Models;

namespace CybersecurityChatbot.Services
{
    // This class controls the main chatbot logic.
    // It checks what the user typed and returns the best cybersecurity response.
    public class ChatbotService
    {
        // Stores all predefined chatbot responses from the ResponseBank
        private readonly List<Response> _responses;

        // Used to randomly choose different responses so the chatbot feels natural
        private readonly Random _random;

        // Stores the current user chatting with the bot
        private User _currentUser;

        // Constructor that prepares the chatbot when the program starts
        public ChatbotService()
        {
            // Load all responses from the response bank
            _responses = ResponseBank.GetResponses();

            // Create the random object for selecting random responses
            _random = new Random();

            // Create a default user
            _currentUser = new User();
        }

        // Sets the current user for the chatbot
        public void SetUser(User user)
        {
            _currentUser = user;
        }

        // Returns the current user object
        public User GetCurrentUser()
        {
            return _currentUser;
        }

        // Checks if the user input is valid
        public bool IsValidInput(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.Trim().Length >= 2;
        }

        // This method receives the user's message and returns the chatbot response
        public string GetResponse(string userInput)
        {
            // If the input is invalid, return an error message
            if (!IsValidInput(userInput))
            {
                return "I did not quite understand that. Please type a full question or message.";
            }

            // Make the input lowercase and easier to compare
            string normalizedInput = NormalizeInput(userInput);

            // Store the last question asked by the user
            _currentUser.LastQuestion = userInput.Trim();

            // Detect the user's mood from the input
            DetectSentiment(normalizedInput);

            // Update memory if the user mentions something important
            UpdateMemory(normalizedInput);

            // Check if the user is asking about remembered information
            string memoryResponse = HandleMemoryPrompts(normalizedInput);
            if (!string.IsNullOrWhiteSpace(memoryResponse))
            {
                return memoryResponse;
            }

            // Check if the user is asking a follow-up question
            string followUpResponse = HandleFollowUp(normalizedInput);
            if (!string.IsNullOrWhiteSpace(followUpResponse))
            {
                return followUpResponse;
            }

            // Loop through all responses and check if any keyword matches the user input
            foreach (var response in _responses)
            {
                if (MatchesAnyKeyword(normalizedInput, response.Keywords))
                {
                    // Choose one random response from the matching category
                    string selected = GetRandomResponse(response);

                    // Save the last topic discussed
                    if (!string.IsNullOrWhiteSpace(response.Category))
                    {
                        _currentUser.LastTopic = response.Category;
                    }

                    // If the user wants to exit, include the user's name in the goodbye message
                    if (response.Category == "Exit")
                    {
                        return selected.Replace("{userName}", _currentUser.Name);
                    }

                    // Return the response with a mood-based message if needed
                    return ApplySentimentPrefix(selected, response.Category);
                }
            }

            // If the user gives a short follow-up, use the previous topic to continue the conversation
            if (!string.IsNullOrWhiteSpace(_currentUser.LastTopic) && normalizedInput.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length <= 4)
            {
                var previousTopicResponse = _responses.FirstOrDefault(r =>
                    string.Equals(r.Category, _currentUser.LastTopic, StringComparison.OrdinalIgnoreCase));

                if (previousTopicResponse != null)
                {
                    return ApplySentimentPrefix(
                        $"It sounds like you are still asking about {_currentUser.LastTopic}. Let me explain further.\n\n{GetRandomResponse(previousTopicResponse)}",
                        _currentUser.LastTopic);
                }
            }

            // If nothing matches, return a default helpful response
            return ApplySentimentPrefix(GetDefaultResponse(), _currentUser.LastTopic);
        }

        // Converts user input to lowercase and removes extra spaces
        private string NormalizeInput(string input)
        {
            return input.ToLower().Trim();
        }

        // Checks if the user's input matches any keyword
        private bool MatchesAnyKeyword(string input, IEnumerable<string> keywords)
        {
            // Go through each keyword one by one
            foreach (string keyword in keywords)
            {
                // Make the keyword lowercase and remove extra spaces
                string normalizedKeyword = keyword.ToLower().Trim();

                // Check if the whole input is exactly the same as the keyword
                if (input == normalizedKeyword)
                {
                    return true;
                }

                // Regex helps match whole words instead of matching only parts of words
                string pattern = $@"\b{Regex.Escape(normalizedKeyword)}\b";
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

            // Return false if no keyword matched
            return false;
        }

        // Selects a random response from the response options
        private string GetRandomResponse(Response response)
        {
            // If no response options exist, return a safe fallback message
            if (response.ResponseOptions == null || response.ResponseOptions.Count == 0)
            {
                return "I am sorry, I do not have a response for that yet.";
            }

            // Return one random response from the list
            return response.ResponseOptions[_random.Next(response.ResponseOptions.Count)];
        }

        // Detects the user's mood based on words in their message
        private void DetectSentiment(string input)
        {
            if (ContainsAny(input, "worried", "scared", "nervous", "afraid", "concerned"))
            {
                _currentUser.CurrentSentiment = "worried";
            }
            else if (ContainsAny(input, "curious", "interested", "want to know", "wondering"))
            {
                _currentUser.CurrentSentiment = "curious";
            }
            else if (ContainsAny(input, "frustrated", "annoyed", "confused", "stressed", "tired"))
            {
                _currentUser.CurrentSentiment = "frustrated";
            }
            else if (ContainsAny(input, "happy", "great", "good", "excited"))
            {
                _currentUser.CurrentSentiment = "positive";
            }
            else
            {
                _currentUser.CurrentSentiment = "neutral";
            }
        }

        // Updates the chatbot memory when the user mentions topics they like
        private void UpdateMemory(string input)
        {
            if (input.Contains("i'm interested in") || input.Contains("i am interested in"))
            {
                string topic = ExtractTopic(input, "interested in");
                SaveFavoriteTopic(topic);
            }

            if (input.Contains("my favourite topic is") || input.Contains("my favorite topic is"))
            {
                string topic = ExtractTopic(input, "topic is");
                SaveFavoriteTopic(topic);
            }

            if (input.Contains("i like"))
            {
                string topic = ExtractTopic(input, "i like");
                SaveFavoriteTopic(topic);
            }
        }

        // Saves the user's favourite cybersecurity topic
        private void SaveFavoriteTopic(string topic)
        {
            if (!string.IsNullOrWhiteSpace(topic))
            {
                _currentUser.FavoriteTopic = topic;
                _currentUser.MemoryFacts["favorite_topic"] = topic;
            }
        }

        // Extracts the topic from the user's sentence
        private string ExtractTopic(string input, string marker)
        {
            int index = input.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return string.Empty;
            }

            return input[(index + marker.Length)..].Trim(' ', '.', '!', '?');
        }

        // Handles questions where the user asks what the chatbot remembers
        private string HandleMemoryPrompts(string input)
        {
            if (ContainsAny(input, "what do you remember", "what do you know about me", "show memory", "remember me"))
            {
                List<string> parts = new()
                {
                    $"Your name is {_currentUser.Name}."
                };

                if (!string.IsNullOrWhiteSpace(_currentUser.FavoriteTopic))
                {
                    parts.Add($"Your favourite cybersecurity topic is {_currentUser.FavoriteTopic}.");
                }

                if (!string.IsNullOrWhiteSpace(_currentUser.LastTopic))
                {
                    parts.Add($"The last topic we discussed was {_currentUser.LastTopic}.");
                }

                parts.Add($"Your current detected mood is {_currentUser.CurrentSentiment}.");

                return string.Join(" ", parts);
            }

            return string.Empty;
        }

        // Handles follow-up questions like "tell me more" or "why"
        private string HandleFollowUp(string input)
        {
            if (ContainsAny(input, "tell me more", "explain more", "another tip", "give me another tip", "what do you mean", "can you simplify that"))
            {
                if (!string.IsNullOrWhiteSpace(_currentUser.LastTopic))
                {
                    var topicResponse = _responses.FirstOrDefault(r =>
                        string.Equals(r.Category, _currentUser.LastTopic, StringComparison.OrdinalIgnoreCase));

                    if (topicResponse != null)
                    {
                        string intro = _currentUser.LastTopic switch
                        {
                            "Password Safety" => "Here is another password tip:",
                            "Phishing" => "Here is more about phishing:",
                            "Privacy" => "Here is more about online privacy:",
                            "Malware" => "Here is more about malware protection:",
                            "Authentication" => "Here is more about two-factor authentication:",
                            _ => $"Here is more about {_currentUser.LastTopic}:"
                        };

                        return ApplySentimentPrefix($"{intro}\n{GetRandomResponse(topicResponse)}", _currentUser.LastTopic);
                    }
                }

                return "I can explain more once we choose a topic. Try asking about passwords, phishing, privacy, malware, or 2FA.";
            }

            if (input == "why" || input.Contains("why is it dangerous") || input.Contains("why is that dangerous") || input.Contains("why is it important"))
            {
                if (!string.IsNullOrWhiteSpace(_currentUser.LastTopic))
                {
                    var topicResponse = _responses.FirstOrDefault(r =>
                        string.Equals(r.Category, _currentUser.LastTopic, StringComparison.OrdinalIgnoreCase));

                    if (topicResponse != null)
                    {
                        return ApplySentimentPrefix(
                            $"That is a great question about {_currentUser.LastTopic}. Here is why it matters:\n\n{GetRandomResponse(topicResponse)}",
                            _currentUser.LastTopic);
                    }
                }

                return "That is a good question. Ask me about a topic first, and then I can explain why it matters.";
            }

            return string.Empty;
        }

        // Adds a suitable introduction based on the user's detected mood
        private string ApplySentimentPrefix(string response, string topic)
        {
            string topicTip = string.Empty;

            if (_currentUser.CurrentSentiment == "worried")
            {
                topicTip = topic switch
                {
                    "Phishing" => "\n\nA quick tip: never click links in urgent emails without checking the sender first.",
                    "Password Safety" => "\n\nA quick tip: start by changing your most important passwords first, like email and banking.",
                    "Privacy" => "\n\nA quick tip: review your account privacy settings one app at a time.",
                    _ => string.Empty
                };

                return "It is completely understandable to feel worried. Let me help you in a calm and practical way.\n\n" + response + topicTip;
            }

            if (_currentUser.CurrentSentiment == "curious")
            {
                return "That is great curiosity. Learning more about cybersecurity is one of the best ways to stay safe.\n\n" + response;
            }

            if (_currentUser.CurrentSentiment == "frustrated")
            {
                return "I understand this can feel frustrating. Let me keep it simple and useful.\n\n" + response + "\n\nLet me simplify it step by step if you want.";
            }

            if (_currentUser.CurrentSentiment == "positive")
            {
                return "That is a great attitude. Let us build on that.\n\n" + response;
            }

            return response;
        }

        // Gives a default response when the chatbot does not understand the input
        private string GetDefaultResponse()
        {
            if (!string.IsNullOrWhiteSpace(_currentUser.FavoriteTopic))
            {
                return $"I am not fully sure what you mean yet, but since you are interested in {_currentUser.FavoriteTopic}, you could ask me more about that topic.";
            }

            return "I am not sure I understand that yet. Try asking about password safety, phishing scams, privacy, safe browsing, malware, or two-factor authentication.";
        }

        // Checks if the input contains any of the given words or phrases
        private bool ContainsAny(string input, params string[] phrases)
        {
            return phrases.Any(p => input.Contains(p));
        }
    }
}