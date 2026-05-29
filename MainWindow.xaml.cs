using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chatbot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Optional: Set a default page to show immediately on startup
            navframe.Navigate(new HomePage());
        }

      




        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            // Changes the hosted page to HomePage
            navframe.Navigate( new HomePage());
        }

        private void BotButton_Click(object sender, RoutedEventArgs e)
        {
            // Inside your MainWindow.xaml.cs where you load the Page:
            Bot chatPage = new Bot();

            // Subscribe to the delegate event using C# lambda operators
            chatPage.ChatSessionEnded += (sender, userName) =>
            {
                MessageBox.Show($"Goodbye, {userName}! Closing app.");
                Application.Current.Shutdown();
            };

            // Changes the hosted page to ChatBotPage
            navframe.Navigate(new Bot());
        }
        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            // Changes the hosted page to GamePage
            navframe.Navigate(new Game());
        }
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            // Changes the hosted page to AboutPage
            navframe.Navigate(new About());
        }
    }
}