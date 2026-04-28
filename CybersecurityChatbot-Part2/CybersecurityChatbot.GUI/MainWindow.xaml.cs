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
    public partial class MainWindow : Window
    {
        private readonly ChatbotService _chatbotService;
        private User _currentUser;

        public MainWindow()
        {
            InitializeComponent();

            _chatbotService = new ChatbotService();
            _currentUser = new User("Guest");
            _chatbotService.SetUser(_currentUser);

            Loaded += MainWindow_Loaded;

            PlayGreeting();
            AppendBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot.");
            AppendBotMessage("Please set your name, then ask me about passwords, phishing, privacy, malware, or 2FA.");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UserInput.Focus();
        }

        private void PlayGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "greeting.wav");
                AudioService audioService = new AudioService(audioPath);
                audioService.PlayGreeting();
            }
            catch
            {
                // Prevent crash if audio file is missing
            }
        }

        private void SetName_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                AppendBotMessage("Please enter a valid name with at least 2 characters.");
                return;
            }

            _currentUser = new User(name);
            _chatbotService.SetUser(_currentUser);

            AppendBotMessage($"Welcome, {name}! I will remember your name during our chat.");
            UserInput.Focus();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
            }
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                UserInput.Text = button.Content.ToString();
                ProcessInput();
            }
        }

        private void ShowMemory_Click(object sender, RoutedEventArgs e)
        {
            AppendBotMessage(_chatbotService.GetResponse("what do you remember"));
            UserInput.Focus();
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            AppendBotMessage("Chat cleared. We can continue from here.");
            UserInput.Focus();
        }

        private void ProcessInput()
        {
            string input = UserInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                AppendBotMessage("Please type something so I can help you.");
                return;
            }

            _currentUser.IncrementMessageCount();
            AppendUserMessage(input);

            string response = _chatbotService.GetResponse(input);
            AppendBotMessage(response);

            UserInput.Clear();
            UserInput.Focus();
        }

        private void AppendUserMessage(string message)
        {
            ChatPanel.Children.Add(CreateMessageBubble($"{_currentUser.Name}: {message}", false));
            ScrollToBottom();
        }

        private void AppendBotMessage(string message)
        {
            ChatPanel.Children.Add(CreateMessageBubble($"BOT: {message}", true));
            ScrollToBottom();
        }

        private Border CreateMessageBubble(string text, bool isBot)
        {
            Border bubble = new Border
            {
                Background = isBot
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F766E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                MaxWidth = 700
            };

            bubble.Child = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };

            return bubble;
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToEnd();
        }

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

        private void HeaderBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
                return;
            }

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