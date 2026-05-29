using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chatbot
{
    public partial class Bot : Page
    {
        // 1. DECLARE THE DELEGATE AND THE EVENT
        public delegate void ChatSessionEndedEventHandler(object sender, string userName);
        public event ChatSessionEndedEventHandler ChatSessionEnded;

        // Enum to track conversation state machine
        private enum ChatState
        {
            AwaitingName,
            AwaitingMood,
            AnsweringTopics
        }

        private ChatState _currentState = ChatState.AwaitingName;
        private string _userName = "Noname";
        private readonly string _botName = "CyberBot";

        // NEW: Field to track the last discussed topic for "tell me" follow-ups
        private string _lastTopic = string.Empty;

        // Dictionaries moved to class-level variables
        private readonly Dictionary<string, string> _moodResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"good","Nice to hear that"},
            {"bad","Cheer up im here for you"},
            {"okay","Cool"},
            {"happy","Wow you must be having a nice day"},
            {"sad","Cheer up it's not the end of the world" },
            {"worried","I'm sorry to hear that gang" },
            {"im good","Nice to hear that"},
            {"im bad","Cheer up im here for you"},
            {"im okay","Cool"},
            {"im happy","Wow you must be having a nice day"},
            {"im sad","Cheer up its not the end of the world" },
            {"im worried","I'm sorry to hear that gang" }
        };

        private readonly List<string> _fallbacks = new List<string>
        {
            "I'm not sure what you mean.",
            "Could you rephrase that?",
            "That's a bit over my head!"
        };

        private readonly Dictionary<string, List<string>> _topics = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "password safety", new List<string>(){
                "Never reuse passwords across different sites; if one site is breached, your other accounts remain secure.",
                "Aim for at least 12–16 characters. Longer is generally stronger.",
                "Do not use personal info (names, birthdays) or common words, as these are easily guessed.",
                "Combine random words into a long sentence, which is easier to remember but harder to crack."
            }},
            { "scams", new List<string>(){
                "Scammers often pose as trusted companies. Always verify email addresses.",
                "Unusual Requests: Asking for bank credentials, passwords, or gift cards.",
                "Unexpected Attachments / Links: Unsolicited documents that can install malware when opened."
            }},
            { "privacy", new List<string>(){
                "Limit what personal information you share online.",
                "Be Cautious with Public Wi-Fi: Avoid logging into bank accounts on public Wi-Fi networks.",
                "Report Scams: Report phishing to your IT team at work or to government bodies like the FTC."
            }},
            { "phishing", new List<string>(){
                "Don’t click on suspicious links or download attachments from unknown sources.",
                "Mismatched/Spoofed Email Addresses: Checking the actual sender email address often reveals slight misspellings."
            }}
        };

        public Bot()
        {
            InitializeComponent();

            // Welcome message & First question
            AddBotMessage($"{_botName}: Welcome! I'm your cybersecurity awareness bot.");
            AddBotMessage($"{_botName}: What is your name?");
        }

        private async void OnButtonClick(object sender, RoutedEventArgs e)
        {
            string cleanInput = txtMessage.Text.Trim();

            if (string.IsNullOrWhiteSpace(cleanInput))
            {
                // UI "Thinking" animation simulation
                var msg1 = AddBotMessage("Thinking.");
                await Task.Delay(400);
                ListChat.Items.Remove(msg1);

                var msg2 = AddBotMessage("Thinking..");
                await Task.Delay(500);
                ListChat.Items.Remove(msg2);

                var msg3 = AddBotMessage("Thinking...");
                await Task.Delay(600);
                ListChat.Items.Remove(msg3);

                MessageBox.Show("Enter a text please");
                return;
            }

            // Post User Message to UI
            AddUserMessage(txtMessage.Text);
            txtMessage.Clear();
            txtMessage.Focus();

            //  "Thinking" delay for realism before bot responds
            var thinkMsg = AddBotMessage("Thinking...");
            await Task.Delay(600);
            ListChat.Items.Remove(thinkMsg);

            // Exit Command Integration
            if (cleanInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                AddBotMessage("Thank you for using the Cybersecurity Awareness Chatbot. Stay safe!");
                await Task.Delay(1500);

                // Raise the delegate event instead of shutting down forcefully
                OnChatSessionEnded();
                return;
            }

            // Process conversation based on state
            ProcessBotLogic(cleanInput);

            // Auto-scrolls to the newest message
            if (ListChat.Items.Count > 0)
            {
                ListChat.ScrollIntoView(ListChat.Items[ListChat.Items.Count - 1]);
            }
        }

        private void ProcessBotLogic(string input)
        {
            Random random = new Random();

            switch (_currentState)
            {
                case ChatState.AwaitingName:
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        _userName = input;
                    }
                    AddBotMessage($"Lets begin {_userName}");
                    AddBotMessage($"{_botName}: How are you?");
                    _currentState = ChatState.AwaitingMood;
                    break;

                case ChatState.AwaitingMood:
                    if (_moodResponses.TryGetValue(input, out string feedback))
                    {
                        AddBotMessage($"{_botName}: {feedback}");
                        AddBotMessage($"{_botName}: {_userName}! Ask me anything about password safety, phishing, scams, and privacy. Type 'exit' to end program.");
                        _currentState = ChatState.AnsweringTopics;
                    }
                    else
                    {
                        string randomFallback = _fallbacks[random.Next(_fallbacks.Count)];
                        AddBotMessage($"{_botName}: {randomFallback}");
                        AddBotMessage($"{_botName}: Please try again. How are you?");
                    }
                    break;

                case ChatState.AnsweringTopics:
                    // NEW: Check if the user typed a variation of "tell me"
                    if (input.Equals("tell me", StringComparison.OrdinalIgnoreCase) ||
                        input.Equals("tell me more", StringComparison.OrdinalIgnoreCase) ||
                        input.Equals("expand", StringComparison.OrdinalIgnoreCase))
                    {
                        // Verify if there is a previous topic stored to talk about
                        if (!string.IsNullOrEmpty(_lastTopic) && _topics.TryGetValue(_lastTopic, out List<string>? dynamicText))
                        {
                            string infoResponse = dynamicText[random.Next(dynamicText.Count)];
                            AddBotMessage($"{_botName}: [More on {_lastTopic}]: {infoResponse}");
                        }
                        else
                        {
                            AddBotMessage($"{_botName}: What topic would you like to hear about first? (password safety, phishing, scams, or privacy)");
                        }
                    }
                    // Regular topic check
                    else if (_topics.TryGetValue(input, out List<string>? specificText))
                    {
                        _lastTopic = input; // Save this topic context for follow-up questions
                        string infoResponse = specificText[random.Next(specificText.Count)];
                        AddBotMessage($"{_botName}: {infoResponse}");
                    }
                    else
                    {
                        AddBotMessage($"{_botName}: I didn't quite understand that. Please try asking about password safety, phishing, scams, or privacy.");
                    }
                    break;
            }
        }

        // HELPER METHOD TO SAFELY CALL THE DELEGATE
        protected virtual void OnChatSessionEnded()
        {
            ChatSessionEnded?.Invoke(this, _userName);
        }

        // Helper for Bot Bubbles (Left Side)
        // Change "private void" to "private ChatBubble"
        private ChatBubble AddBotMessage(string text)
        {
            var bubble = new ChatBubble
            {
                Text = text,
                Alignment = "Left",
                BackgroundColor = "#EAEAEA",
                ForegroundColor = "Black"
            };
            ListChat.Items.Add(bubble);

            return bubble; //  CRITICAL: This returns the object so the UI can remove it later
        }


        // Helper for User Bubbles (Right Side)
        private void AddUserMessage(string text)
        {
            var bubble = new ChatBubble
            {
                Text = text,
                Alignment = "Right",
                BackgroundColor = "#007ACC",
                ForegroundColor = "White"
            };
            ListChat.Items.Add(bubble);
        }

        public class ChatBubble
        {
            public string Text { get; set; } = string.Empty;
            public string Alignment { get; set; } = string.Empty;
            public string BackgroundColor { get; set; } = string.Empty;
            public string ForegroundColor { get; set; } = string.Empty;
        }
    }
}
