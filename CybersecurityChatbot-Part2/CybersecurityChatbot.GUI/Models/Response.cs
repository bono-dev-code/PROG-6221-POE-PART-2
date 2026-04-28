using System.Collections.Generic;

namespace CybersecurityChatbot.Models
{
    /// <summary>
    /// Represents a chatbot response with keywords and multiple response options.
    /// </summary>
    public class Response
    {
        public string[] Keywords { get; set; }
        public List<string> ResponseOptions { get; set; }
        public string Category { get; set; }

        public Response(string[] keywords, List<string> responseOptions, string category = "General")
        {
            Keywords = keywords;
            ResponseOptions = responseOptions;
            Category = category;
        }
    }

    /// <summary>
    /// Static response bank for the chatbot.
    /// </summary>
    public static class ResponseBank
    {
        public static List<Response> GetResponses()
        {
            return new List<Response>
            {
                new Response(
                    new[] { "hello", "hi", "hey", "greetings" },
                    new List<string>
                    {
                        "Hello! Welcome to the Cybersecurity Awareness Bot. I am ready to help you stay safe online today.",
                        "Hi there! I am your Cybersecurity Awareness Assistant. Ask me anything about online safety.",
                        "Hey! I am here to guide you through important cybersecurity tips and safe online habits."
                    },
                    "Greeting"
                ),

                new Response(
                    new[] { "how are you", "how do you do" },
                    new List<string>
                    {
                        "I am doing great, thank you for asking. I am ready to help you learn how to stay safe online.",
                        "I am doing well. My goal is to help you understand cybersecurity in a simple and useful way."
                    },
                    "Greeting"
                ),

                new Response(
                    new[] { "purpose", "what can you do", "what do you do", "your purpose", "what is your purpose" },
                    new List<string>
                    {
                        "I am your Cybersecurity Awareness Assistant. I can help you with password safety, phishing, scams, safe browsing, malware, social engineering, suspicious links, online privacy, and two-factor authentication.",
                        "My purpose is to educate users about cybersecurity threats and teach safe online practices in a simple conversation."
                    },
                    "Purpose"
                ),

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

                new Response(
                    new[] { "password manager", "password vault" },
                    new List<string>
                    {
                        "A password manager stores your passwords securely in one place. It also helps generate strong and unique passwords so you do not need to reuse the same one everywhere.",
                        "Using a password manager is one of the best ways to improve password security. You only need to remember one strong master password."
                    },
                    "Password Safety"
                ),

                new Response(
                    new[]
                    {
                        "2fa",
                        "two factor authentication",
                        "two-factor authentication",
                        "multi factor authentication",
                        "multi-factor authentication",
                        "mfa",
                        "authentication",
                        "otp",
                        "one time pin",
                        "verification code"
                    },
                    new List<string>
                    {
                        "TWO-FACTOR AUTHENTICATION:\n" +
                        "Two-factor authentication adds an extra layer of security to your accounts.\n\n" +
                        "Even if someone steals your password, they still need a second verification step, such as a code sent to your phone, to log in.\n\n" +
                        "You should enable 2FA on:\n" +
                        "• Email accounts\n" +
                        "• Banking apps\n" +
                        "• Social media\n" +
                        "• Shopping accounts",

                        "Using two-factor authentication, also called 2FA, is one of the best ways to protect your accounts. Even if your password is compromised, attackers cannot easily log in without the second verification step."
                    },
                    "Authentication"
                ),

                new Response(
                    new[] { "phishing", "phishing scams", "phish", "scam", "scams", "scam email", "fake email" },
                    new List<string>
                    {
                        "PHISHING AWARENESS:\n" +
                        "Phishing is when criminals pretend to be trusted people or companies to trick you into giving away passwords, banking details, or other personal information.\n\n" +
                        "Warning signs include:\n" +
                        "• Urgent or threatening language\n" +
                        "• Strange sender addresses\n" +
                        "• Suspicious links\n" +
                        "• Requests for personal information\n" +
                        "• Poor spelling and grammar",

                        "If a message pressures you to click quickly, confirm account details, or send personal information, it may be phishing. Stop, verify the sender, and never rush."
                    },
                    "Phishing"
                ),

                new Response(
                    new[] { "safe browsing", "browsing", "internet safety", "online safety", "browse safely" },
                    new List<string>
                    {
                        "SAFE BROWSING TIPS:\n" +
                        "1. Check for HTTPS before entering personal information.\n" +
                        "2. Avoid downloading files from unknown websites.\n" +
                        "3. Keep your browser updated.\n" +
                        "4. Do not click suspicious pop-ups.\n" +
                        "5. Avoid sensitive transactions on public Wi-Fi.",

                        "Safe browsing means thinking before you click. Always check website legitimacy, avoid suspicious downloads, and keep your browser and device updated."
                    },
                    "Safe Browsing"
                ),

                new Response(
                    new[] { "malware", "virus", "trojan", "ransomware", "worm" },
                    new List<string>
                    {
                        "MALWARE PROTECTION:\n" +
                        "Malware is harmful software that can steal data, damage files, or spy on you. Keep antivirus software updated, avoid suspicious downloads, and never open unknown attachments.",
                        "Signs of malware can include a slow computer, strange pop-ups, missing files, or programs opening on their own. Regular updates and backups help protect you."
                    },
                    "Malware"
                ),

                new Response(
                    new[] { "social engineering", "manipulation", "pretexting", "baiting" },
                    new List<string>
                    {
                        "SOCIAL ENGINEERING:\n" +
                        "Social engineering is when attackers manipulate people into revealing sensitive information. They often pretend to be trusted people, support staff, or institutions.",
                        "Always verify before sharing information. A convincing story, urgent request, or emotional pressure is often a sign of social engineering."
                    },
                    "Social Engineering"
                ),

                new Response(
                    new[] { "privacy", "online privacy", "protect my privacy" },
                    new List<string>
                    {
                        "ONLINE PRIVACY TIPS:\n" +
                        "Review privacy settings on your social media accounts, share less personal information publicly, use strong passwords, and enable two-factor authentication.",
                        "Protecting your privacy means controlling what you share, who can see it, and how your accounts are secured."
                    },
                    "Privacy"
                ),

                new Response(
                    new[] { "link", "links", "suspicious link", "unsafe link" },
                    new List<string>
                    {
                        "Before clicking a link, ask yourself: Do I trust the sender? Does the address look correct? Is the message trying to scare or rush me? If something feels wrong, do not click.",
                        "Hover over a link before clicking it. If the real destination looks strange or unrelated to the message, it may be unsafe."
                    },
                    "Suspicious Links"
                ),

                new Response(
                    new[] { "identity theft", "stolen identity", "identity" },
                    new List<string>
                    {
                        "Identity theft happens when criminals steal your personal information and use it fraudulently. Protect your ID number, passwords, banking information, and one-time pins.",
                        "To reduce identity theft risk, do not overshare online, use strong passwords, and be careful with forms, fake calls, and suspicious messages."
                    },
                    "Identity Theft"
                ),

                new Response(
                    new[] { "south africa", "sa", "south african", "cybercrime act", "sim swap" },
                    new List<string>
                    {
                        "SOUTH AFRICAN CYBERSECURITY:\n" +
                        "South Africa faces online banking scams, phishing, SIM swap fraud, fake job scams, and social media impersonation. Citizens should be extra cautious with banking alerts, OTP requests, and calls asking for personal details.",
                        "In South Africa, SIM swap fraud and banking scams are common. Never share one-time pins, account details, or passwords with anyone over the phone."
                    },
                    "South Africa"
                ),

                new Response(
                    new[] { "bye", "goodbye", "exit", "quit", "thank you", "thanks" },
                    new List<string>
                    {
                        "Thank you for using the Cybersecurity Awareness Bot. Stay safe online, {userName}!",
                        "Goodbye, {userName}. Remember to think before you click and protect your information online."
                    },
                    "Exit"
                )
            };
        }
    }
}