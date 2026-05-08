# Cybersecurity Awareness Chatbot (Part 2)

**Name:** Nenguda Bono  
**Student ID:** ST10484954  
**Course:** PROG6221 – Programming 2A  
**Institution:** Rosebank College  
**Project:** POE Part 2  

## Overview

This project extends the Part 1 console chatbot into a **WPF GUI application**. The chatbot now provides a more user-friendly interface while preserving the key features from Part 1, including the **voice greeting**, **cybersecurity-themed ASCII art**, **keyword recognition**, and **dynamic responses**.

The application also adds new Part 2 features:
- memory and recall
- sentiment detection
- conversation flow for follow-up questions
- a polished graphical layout ready for Part 3 extension

## Features

- **GUI built with WPF**
- **Voice greeting** using `greeting.wav`
- **ASCII art translated into the GUI**
- **Keyword recognition** for cybersecurity topics
- **Random responses** for more natural interaction
- **Conversation flow** for prompts like `tell me more` and `another tip`
- **Memory and recall** for user name, favourite topic, and last topic
- **Sentiment detection** for worried, curious, and frustrated moods
- **Quick action buttons** for password, phishing, privacy, and 2FA
- **Structured code** using models and services

## Project Structure

```text
CybersecurityChatbot-Part2/
├── CybersecurityChatbot.GUI/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── CybersecurityChatbot.GUI.csproj
│   ├── Models/
│   │   ├── User.cs
│   │   └── Response.cs
│   ├── Services/
│   │   ├── AudioService.cs
│   │   └── ChatbotService.cs
│   └── Resources/
│       └── greeting.wav
├── .github/
│   └── workflows/
│       └── dotnet.yml
└── README.md
```

## Requirements

- Visual Studio 2022 or later
- .NET 8 SDK
- Windows OS for WPF and WAV audio playback

## How to Run

1. Open `CybersecurityChatbot.GUI.csproj` in Visual Studio.
2. Restore packages if prompted.
3. Build the project.
4. Run the WPF application.
5. Set your name and start chatting.

## Release Tags

For Part 2, releases tags:
- `v1.0 - Initial GUI Chatbot Version`
- `v1.1 - Dynamic Chatbot Responses`
- `v2.0 - Final POE Part 2 Submission`

## Notes

The GitHub Actions workflow in this zip uses **windows-latest** because WPF projects require a Windows runner.
