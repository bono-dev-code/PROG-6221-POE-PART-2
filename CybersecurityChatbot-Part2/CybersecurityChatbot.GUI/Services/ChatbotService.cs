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

            // Smart intent detection checks the specific meaning of the user's question first.
            // This stops the chatbot from sounding confused when broad words overlap,
            // for example password, scam, hack, privacy, Wi-Fi, or malware.
            string smartIntentResponse = DetectSmartIntent(normalizedInput);
            if (!string.IsNullOrWhiteSpace(smartIntentResponse))
            {
                return smartIntentResponse;
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

        // Detects specific cybersecurity intentions before the general ResponseBank search.
        // This method improves the NLP simulation requirement by allowing the chatbot
        // to understand different ways a user may phrase the same cybersecurity problem.
        private string DetectSmartIntent(string input)
        {
            // HACKED ACCOUNT / COMPROMISED ACCOUNT
            if (ContainsAny(input,
                "account hacked", "my account was hacked", "someone hacked my account",
                "account compromised", "someone logged into my account", "cannot access my account",
                "my facebook was hacked", "my instagram was hacked", "my email was hacked",
                "my account got stolen", "hacked account"))
            {
                _currentUser.LastTopic = "Hacked Account";
                return ApplySentimentPrefix(
                    "It sounds like your account may be compromised. Change your password immediately, enable two-factor authentication, log out of all devices, and check your recovery email and phone number. If money or banking is involved, contact your bank as soon as possible.",
                    _currentUser.LastTopic);
            }

            // SUSPICIOUS LINK / BAD LINK
            if (ContainsAny(input,
                "clicked a suspicious link", "clicked a bad link", "opened a suspicious link",
                "dangerous link", "suspicious link", "bad link", "unknown link",
                "clicked a link", "link from sms", "link from whatsapp", "link looks fake"))
            {
                _currentUser.LastTopic = "Suspicious Links";
                return ApplySentimentPrefix(
                    "If you clicked a suspicious link, do not enter any personal details. Close the page, change your password if you typed it in, scan your device, and enable two-factor authentication on important accounts.",
                    _currentUser.LastTopic);
            }

            // PASSWORD LEAKED / STOLEN PASSWORD
            if (ContainsAny(input,
                "password leaked", "my password got leaked", "leaked password",
                "password stolen", "stolen password", "password hacked", "data breach",
                "password exposed", "my login was leaked", "credentials leaked"))
            {
                _currentUser.LastTopic = "Password Leak";
                return ApplySentimentPrefix(
                    "If your password was leaked, change it immediately. Do not reuse the old password, enable two-factor authentication, and check other accounts where you used the same password.",
                    _currentUser.LastTopic);
            }

            // STRONG PASSWORD / PASSWORD STRENGTH
            if (ContainsAny(input,
                "strong password", "make my password stronger", "password stronger",
                "secure password", "better password", "password strength", "weak password",
                "create a password", "good password", "safe password", "hard to guess password"))
            {
                _currentUser.LastTopic = "Password Safety";
                return ApplySentimentPrefix(
                    "A strong password should be long, unique, and difficult to guess. Use at least 12 characters, mix uppercase letters, lowercase letters, numbers, and symbols, and avoid names, birthdays, or phone numbers.",
                    _currentUser.LastTopic);
            }

            // PASSWORD SHARING
            if (ContainsAny(input,
                "share password", "share my password", "send password", "give password",
                "whatsapp password", "email password", "someone asked for my password",
                "bank asked for my password", "send my login"))
            {
                _currentUser.LastTopic = "Password Sharing";
                return ApplySentimentPrefix(
                    "Never share your password through WhatsApp, SMS, email, or phone calls. A real company or bank should never ask for your password. If someone asks for it, treat it as a warning sign.",
                    _currentUser.LastTopic);
            }

            // PHISHING EMAIL
            if (ContainsAny(input,
                "fake email", "phishing email", "scam email", "suspicious email",
                "email asking for password", "email asking for banking details",
                "email looks fake", "email scam", "strange email", "email attachment"))
            {
                _currentUser.LastTopic = "Phishing";
                return ApplySentimentPrefix(
                    "That sounds like a phishing attempt. Do not click links or download attachments. Check the sender address carefully, avoid entering passwords from email links, and report the email as phishing if it looks suspicious.",
                    _currentUser.LastTopic);
            }

            // BANKING / OTP SCAM
            if (ContainsAny(input,
                "otp", "bank scam", "banking scam", "stole my otp",
                "someone asked for my otp", "fake bank message", "banking details",
                "card number", "pin number", "capitec scam", "fnb scam", "absa scam",
                "standard bank scam", "bank sms", "bank whatsapp"))
            {
                _currentUser.LastTopic = "Banking Scam";
                return ApplySentimentPrefix(
                    "Never share your OTP, PIN, card number, or banking password. Banks will not ask for your OTP by phone, SMS, WhatsApp, or email. If you shared it, contact your bank immediately and block suspicious transactions.",
                    _currentUser.LastTopic);
            }

            // SIM SWAP FRAUD
            if (ContainsAny(input,
                "sim swap", "sim card hacked", "sim cloned", "lost signal suddenly",
                "phone has no signal", "sim fraud", "number stolen", "cellphone number stolen"))
            {
                _currentUser.LastTopic = "SIM Swap Fraud";
                return ApplySentimentPrefix(
                    "SIM swap fraud happens when criminals move your phone number to another SIM card to receive your OTPs. If your signal disappears unexpectedly, contact your mobile network and bank immediately.",
                    _currentUser.LastTopic);
            }

            // PUBLIC WIFI / FREE WIFI
            if (ContainsAny(input,
                "public wifi", "public wi-fi", "free wifi", "free wi-fi",
                "airport wifi", "coffee shop wifi", "wifi safe", "wi-fi safe",
                "public network", "unsecured wifi", "wifi at mall"))
            {
                _currentUser.LastTopic = "Public Wi-Fi";
                return ApplySentimentPrefix(
                    "Public Wi-Fi can be risky because attackers may intercept your information. Avoid banking on public Wi-Fi, do not enter sensitive passwords, and use a trusted VPN where possible.",
                    _currentUser.LastTopic);
            }

            // MALWARE / VIRUS / SPYWARE
            if (ContainsAny(input,
                "malware", "virus", "computer infected", "phone infected",
                "laptop acting weird", "device acting weird", "spyware", "trojan",
                "computer has a virus", "phone has a virus", "strange popups", "unknown app"))
            {
                _currentUser.LastTopic = "Malware";
                return ApplySentimentPrefix(
                    "Malware is harmful software that can steal data, damage files, or spy on you. Run an antivirus scan, remove suspicious apps, update your system, and avoid downloading unknown files.",
                    _currentUser.LastTopic);
            }

            // RANSOMWARE
            if (ContainsAny(input,
                "ransomware", "files encrypted", "locked files", "pay hackers",
                "computer locked", "data locked", "files locked", "ransom message"))
            {
                _currentUser.LastTopic = "Ransomware";
                return ApplySentimentPrefix(
                    "Ransomware locks or encrypts your files and demands payment. Disconnect from the internet, do not rush to pay, report the attack, and restore from a safe backup if available.",
                    _currentUser.LastTopic);
            }

            // PRIVACY / TRACKING
            if (ContainsAny(input,
                "apps tracking me", "websites tracking me", "protect my privacy",
                "online privacy", "personal information", "data privacy", "app permissions",
                "social media privacy", "who can see my information", "stop tracking me"))
            {
                _currentUser.LastTopic = "Privacy";
                return ApplySentimentPrefix(
                    "To protect your privacy, limit app permissions, use strong privacy settings, avoid oversharing personal details, and review what information websites and apps can collect from you.",
                    _currentUser.LastTopic);
            }

            // WHATSAPP SCAM / VERIFICATION CODE
            if (ContainsAny(input,
                "whatsapp scam", "whatsapp hacked", "verification code", "whatsapp code",
                "someone asked for my whatsapp code", "whatsapp verification", "whatsapp account stolen"))
            {
                _currentUser.LastTopic = "WhatsApp Scam";
                return ApplySentimentPrefix(
                    "WhatsApp scams often involve criminals asking for your verification code. Never share that code. Enable two-step verification in WhatsApp and warn your contacts if your account is compromised.",
                    _currentUser.LastTopic);
            }

            // ONLINE SHOPPING SCAM / FAKE WEBSITE
            if (ContainsAny(input,
                "fake shop", "fake store", "online shopping scam", "shopping website",
                "too cheap", "fake website", "fake online store", "online seller scam",
                "buying online", "website looks fake"))
            {
                _currentUser.LastTopic = "Online Shopping Scam";
                return ApplySentimentPrefix(
                    "Before buying online, check reviews, website spelling, secure payment options, and contact details. Be careful of deals that look too cheap because fake shops often use unrealistic discounts.",
                    _currentUser.LastTopic);
            }

            // SOCIAL ENGINEERING / IMPERSONATION
            if (ContainsAny(input,
                "social engineering", "someone pretending", "pretending to be my bank",
                "tricked me", "manipulate", "impersonating", "fake support agent",
                "pretending to be support", "pretending to be police"))
            {
                _currentUser.LastTopic = "Social Engineering";
                return ApplySentimentPrefix(
                    "Social engineering is when criminals manipulate people into sharing information or taking unsafe actions. Always verify requests through official channels before trusting messages or calls.",
                    _currentUser.LastTopic);
            }

            // VPN
            if (ContainsAny(input,
                "vpn", "virtual private network", "hide my ip", "secure my connection",
                "protect my connection", "encrypted connection"))
            {
                _currentUser.LastTopic = "VPN";
                return ApplySentimentPrefix(
                    "A VPN helps protect your connection by encrypting your internet traffic, especially on public Wi-Fi. However, you still need strong passwords, updates, and safe browsing habits.",
                    _currentUser.LastTopic);
            }

            // TWO-FACTOR AUTHENTICATION / MFA
            if (ContainsAny(input,
                "2fa", "two factor", "two-factor", "mfa", "multi factor",
                "authentication app", "authenticator app", "login code", "security code"))
            {
                _currentUser.LastTopic = "Authentication";
                return ApplySentimentPrefix(
                    "Two-factor authentication adds an extra security step after your password. Use an authenticator app where possible, and never share login codes or OTPs with anyone.",
                    _currentUser.LastTopic);
            }

            // SAFE BROWSING / WEBSITE SAFETY
            if (ContainsAny(input,
                "safe browsing", "browse safely", "safe website", "unsafe website",
                "website safe", "https", "browser warning", "website security"))
            {
                _currentUser.LastTopic = "Safe Browsing";
                return ApplySentimentPrefix(
                    "To browse safely, check for HTTPS, avoid suspicious pop-ups, do not download unknown files, and be careful with websites that pressure you to act urgently.",
                    _currentUser.LastTopic);
            }

            // CYBERBULLYING / ONLINE HARASSMENT
            if (ContainsAny(input,
                "cyberbullying", "online bullying", "online harassment", "someone is threatening me",
                "harassing me online", "abusive messages", "bullying messages"))
            {
                _currentUser.LastTopic = "Cyberbullying";
                return ApplySentimentPrefix(
                    "Cyberbullying should be taken seriously. Save evidence, block the person, report the account on the platform, and speak to a trusted person or authority if threats are involved.",
                    _currentUser.LastTopic);
            }

            // IDENTITY THEFT
            if (ContainsAny(input,
                "identity theft", "stolen identity", "someone used my id",
                "someone used my information", "my personal details were stolen",
                "id number stolen", "fraud in my name"))
            {
                _currentUser.LastTopic = "Identity Theft";
                return ApplySentimentPrefix(
                    "Identity theft happens when someone uses your personal information without permission. Secure your accounts, contact your bank if needed, report the fraud, and monitor your accounts for suspicious activity.",
                    _currentUser.LastTopic);
            }

            // No specific smart intent was detected.
            // The chatbot will continue to the normal ResponseBank keyword search.
            return string.Empty;
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