using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CybersecurityChatbot.Models;
using CybersecurityChatbot.Services;

namespace CybersecurityChatbot.GUI
{
    // This class controls the main chatbot window.
    // It handles the user interface, buttons, messages, name setup, and chatbot responses.
    public partial class MainWindow : Window
    {
        // Chatbot service used to process user input and return responses
        private readonly ChatbotService _chatbotService;

        // Stores the current user using the chatbot
        private User _currentUser;

        // Constructor that runs when the main window opens
        public MainWindow()
        {
            // Loads all visual components from the XAML file
            InitializeComponent();

            // Create the chatbot service
            _chatbotService = new ChatbotService();

            // Create a default user called Guest
            _currentUser = new User("Guest");

            // Send the current user to the chatbot service
            _chatbotService.SetUser(_currentUser);

            // Run this method when the window has loaded
            Loaded += MainWindow_Loaded;

            // Play the greeting audio
            PlayGreeting();

            // Show welcome messages when the app starts
            AppendBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot.");
            AppendBotMessage("Please set your name, then ask me about passwords, phishing, privacy, malware, or 2FA.");
        }

        // This method runs after the window has loaded
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Put the cursor inside the user input box
            UserInput.Focus();
        }

        // This method plays the greeting audio file
        private void PlayGreeting()
        {
            try
            {
                // Build the path to the greeting audio file
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "greeting.wav");

                // Create the audio service and pass the audio file path
                AudioService audioService = new AudioService(audioPath);

                // Play the greeting audio
                audioService.PlayGreeting();
            }
            catch
            {
                // Prevent the app from crashing if the audio file is missing or fails
            }
        }

        // This method runs when the user clicks the Set Name button
        private void SetName_Click(object sender, RoutedEventArgs e)
        {
            // Get the name entered by the user
            string name = NameInput.Text.Trim();

            // Validate the name before saving it
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                AppendBotMessage("Please enter a valid name with at least 2 characters.");
                return;
            }

            // Create a new user using the entered name
            _currentUser = new User(name);

            // Update the chatbot service with the new user
            _chatbotService.SetUser(_currentUser);

            // Confirm that the chatbot has saved the user's name
            AppendBotMessage($"Welcome, {name}! I will remember your name during our chat.");

            // Put the cursor back into the message input box
            UserInput.Focus();
        }

        // This method runs when the Send button is clicked
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        // This method allows the user to press Enter to send a message
        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
            }
        }

        // This method handles quick topic buttons like password, phishing, and privacy
        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            // Check if the clicked object is a button
            if (sender is Button button)
            {
                // Put the button text into the input box
                UserInput.Text = button.Content.ToString();

                // Process the button text as user input
                ProcessInput();
            }
        }

        // This method shows what the chatbot remembers about the user
        private void ShowMemory_Click(object sender, RoutedEventArgs e)
        {
            AppendBotMessage(_chatbotService.GetResponse("what do you remember"));
            UserInput.Focus();
        }

        // This method clears all messages from the chat panel
        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            AppendBotMessage("Chat cleared. We can continue from here.");
            UserInput.Focus();
        }

        // This method processes the user's typed message
        private void ProcessInput()
        {
            // Get the message from the input box
            string input = UserInput.Text.Trim();

            // Check if the input box is empty
            if (string.IsNullOrWhiteSpace(input))
            {
                AppendBotMessage("Please type something so I can help you.");
                return;
            }

            // Increase the number of messages exchanged
            _currentUser.IncrementMessageCount();

            // Display the user's message in the chat
            AppendUserMessage(input);

            // Get the chatbot response
            string response = _chatbotService.GetResponse(input);

            // Display the chatbot response in the chat
            AppendBotMessage(response);

            // Clear the input box after sending
            UserInput.Clear();

            // Put the cursor back in the input box
            UserInput.Focus();
        }

        // This method displays the user's message in the chat area
        private void AppendUserMessage(string message)
        {
            ChatPanel.Children.Add(CreateMessageBubble($"{_currentUser.Name}: {message}", false));
            ScrollToBottom();
        }

        // This method displays the bot's message in the chat area
        private void AppendBotMessage(string message)
        {
            ChatPanel.Children.Add(CreateMessageBubble($"BOT: {message}", true));
            ScrollToBottom();
        }

        // This method creates the message bubble style for both user and bot messages
        private Border CreateMessageBubble(string text, bool isBot)
        {
            // Create a new border that will act as a message bubble
            Border bubble = new Border
            {
                // Use different background colours for bot and user messages
                Background = isBot
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F766E")),

                // Make the corners rounded
                CornerRadius = new CornerRadius(12),

                // Add inside spacing
                Padding = new Thickness(12),

                // Add spacing between messages
                Margin = new Thickness(0, 0, 0, 10),

                // Bot messages go left, user messages go right
                HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right,

                // Limit the width of the bubble
                MaxWidth = 700
            };

            // Add text inside the message bubble
            bubble.Child = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };

            return bubble;
        }

        // This method keeps the latest message visible
        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToEnd();
        }

        // This method allows the window to be dragged by holding the left mouse button
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                }
                catch
                {
                    // Avoid crash during drag
                }
            }
        }

        // This method allows the header area to drag or maximise the window
        private void HeaderBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Double-clicking the header changes between maximised and normal window size
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
                return;
            }

            // Drag the window when the left mouse button is pressed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    DragMove();
                }
                catch
                {
                    // Avoid crash during drag
                }
            }
        }
    }
}