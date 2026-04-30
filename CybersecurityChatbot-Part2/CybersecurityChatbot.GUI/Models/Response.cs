using System.Collections.Generic;

namespace CybersecurityChatbot.Models
{
    // This class represents a single chatbot response.
    // It stores keywords, possible replies, and a category for organization.
    public class Response
    {
        // Keywords that the chatbot will look for in user input
        public string[] Keywords { get; set; }

        // List of possible responses the chatbot can choose from
        public List<string> ResponseOptions { get; set; }

        // Category to group similar responses (e.g., Greeting, Help, Security)
        public string Category { get; set; }

        // Constructor used to create a new Response object
        public Response(string[] keywords, List<string> responseOptions, string category = "General")
        {
            // Assign the keywords to the property
            Keywords = keywords;

            // Assign the response options to the property
            ResponseOptions = responseOptions;

            // Assign the category (default is "General" if not provided)
            Category = category;
        }
    }

    // This static class stores all predefined chatbot responses.
    // It acts like a "database" of responses for the chatbot.
    public static class ResponseBank
    {
        // This method returns a list of all chatbot responses
        public static List<Response> GetResponses()
        {
            // Create and return a list of Response objects
            return new List<Response>
            {
                // Greeting responses
                new Response(
                    new[] { "hello", "hi", "hey", "greetings" }, // Keywords
                    new List<string> // Possible replies
                    {
                        "Hello! Welcome to the Cybersecurity Awareness Bot. I am ready to help you stay safe online today.",
                        "Hi there! I am your Cybersecurity Awareness Assistant. Ask me anything about online safety.",
                        "Hey! I am here to guide you through important cybersecurity tips and safe online habits."
                    },
                    "Greeting" // Category
                ),

                // Asking how the bot is doing
                new Response(
                    new[] { "how are you", "how do you do" },
                    new List<string>
                    {
                        "I am doing great, thank you for asking. I am ready to help you learn how to stay safe online.",
                        "I am doing well. My goal is to help you understand cybersecurity in a simple and useful way."
                    },
                    "Greeting"
                ),

                // Bot purpose
                new Response(
                    new[] { "purpose", "what can you do", "what do you do", "your purpose", "what is your purpose" },
                    new List<string>
                    {
                        "I am your Cybersecurity Awareness Assistant. I can help you with password safety, phishing, scams, safe browsing, malware, social engineering, suspicious links, online privacy, and two-factor authentication.",
                        "My purpose is to educate users about cybersecurity threats and teach safe online practices in a simple conversation."
                    },
                    "Purpose"
                ),

                // Help topics
                new Response(
                    new[] { "what can i ask you about", "topics", "help", "help me", "more topics", "what else" },
                    new List<string>
                    {
                        "You can ask me about:\n" +
                        "• Password safety\n" +
                        "• Phishing and scams\n" +
                        "• Safe browsing\n" +
                        "• Malware protection\n" +
                        "• Social engineering\n" +
                        "• Two-factor authentication\n" +
                        "• Suspicious links\n" +
                        "• Online privacy\n" +
                        "• South African cyber threats",

                        "I can help with many cybersecurity topics such as passwords, phishing, malware, privacy, social engineering, suspicious links, safe browsing, and 2FA."
                    },
                    "Help"
                ),

                // Password safety
                new Response(
                    new[] { "password", "passwords", "strong password", "secure password", "password safety" },
                    new List<string>
                    {
                        "PASSWORD SAFETY TIPS:\n" +
                        "1. Use at least 12 characters.\n" +
                        "2. Mix uppercase letters, lowercase letters, numbers, and symbols.\n" +
                        "3. Do not reuse passwords across accounts.\n" +
                        "4. Avoid personal details like your name or birthday.\n" +
                        "5. Use a password manager where possible.",

                        "A strong password should be long, unique, and difficult to guess. Avoid names, dates of birth, phone numbers, and common words. A password manager can help you create and store strong passwords safely."
                    },
                    "Password Safety"
                ),

                // Password manager explanation
                new Response(
                    new[] { "password manager", "password vault" },
                    new List<string>
                    {
                        "A password manager stores your passwords securely in one place. It also helps generate strong and unique passwords so you do not need to reuse the same one everywhere.",
                        "Using a password manager is one of the best ways to improve password security. You only need to remember one strong master password."
                    },
                    "Password Safety"
                ),

                // Two-factor authentication
                new Response(
                    new[]
                    {
                        "2fa", "two factor authentication", "two-factor authentication",
                        "multi factor authentication", "multi-factor authentication",
                        "mfa", "authentication", "otp", "one time pin", "verification code"
                    },
                    new List<string>
                    {
                        "TWO-FACTOR AUTHENTICATION:\n" +
                        "Two-factor authentication adds an extra layer of security to your accounts.\n\n" +
                        "Even if someone steals your password, they still need a second verification step.",

                        "Using two-factor authentication is one of the best ways to protect your accounts."
                    },
                    "Authentication"
                ),

                // Phishing awareness
                new Response(
                    new[] { "phishing", "phishing scams", "phish", "scam", "scams", "scam email", "fake email" },
                    new List<string>
                    {
                        "Phishing is when criminals pretend to be trusted sources to steal your information.",
                        "If a message pressures you to act quickly, it may be a phishing attempt."
                    },
                    "Phishing"
                ),

                // Safe browsing
                new Response(
                    new[] { "safe browsing", "browsing", "internet safety", "online safety", "browse safely" },
                    new List<string>
                    {
                        "Always check website safety before entering personal information.",
                        "Avoid clicking unknown links and keep your browser updated."
                    },
                    "Safe Browsing"
                ),

                // Malware
                new Response(
                    new[] { "malware", "virus", "trojan", "ransomware", "worm" },
                    new List<string>
                    {
                        "Malware is harmful software that can damage your system or steal data.",
                        "Keep antivirus software updated and avoid suspicious downloads."
                    },
                    "Malware"
                ),

                // Social engineering
                new Response(
                    new[] { "social engineering", "manipulation", "pretexting", "baiting" },
                    new List<string>
                    {
                        "Social engineering tricks people into giving away sensitive information.",
                        "Always verify before sharing personal details."
                    },
                    "Social Engineering"
                ),

                // Privacy
                new Response(
                    new[] { "privacy", "online privacy", "protect my privacy" },
                    new List<string>
                    {
                        "Review your privacy settings and avoid oversharing online.",
                        "Protect your personal information with strong security practices."
                    },
                    "Privacy"
                ),

                // Suspicious links
                new Response(
                    new[] { "link", "links", "suspicious link", "unsafe link" },
                    new List<string>
                    {
                        "Always check links before clicking them.",
                        "If something feels suspicious, do not click the link."
                    },
                    "Suspicious Links"
                ),

                // Identity theft
                new Response(
                    new[] { "identity theft", "stolen identity", "identity" },
                    new List<string>
                    {
                        "Identity theft happens when your personal information is stolen.",
                        "Protect your details and avoid sharing sensitive information."
                    },
                    "Identity Theft"
                ),

                // South African context
                new Response(
                    new[] { "south africa", "sa", "south african", "cybercrime act", "sim swap" },
                    new List<string>
                    {
                        "In South Africa, scams like SIM swap and phishing are common.",
                        "Never share OTPs or banking details with anyone."
                    },
                    "South Africa"
                ),

                // Exit messages
                new Response(
                    new[] { "bye", "goodbye", "exit", "quit", "thank you", "thanks" },
                    new List<string>
                    {
                        "Thank you for using the Cybersecurity Awareness Bot. Stay safe online, {userName}!",
                        "Goodbye, {userName}. Remember to stay safe online."
                    },
                    "Exit"
                )
            };
        }
    }
}