using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;


namespace Chatbot
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();

            // Start the video loop automatically on startup
            BackgroundVideo.Play();

            AsciiDisplay.Text = @"  /$$$$$$            /$$                                 /$$$$$$$              /$$    
 /$$__  $$          | $$                                | $$__  $$            | $$    
| $$  \__/ /$$   /$$| $$$$$$$   /$$$$$$   /$$$$$$       | $$  \ $$  /$$$$$$  /$$$$$$  
| $$      | $$  | $$| $$__  $$ /$$__  $$ /$$__  $$      | $$$$$$$  /$$__  $$|_  $$_/  
| $$      | $$  | $$| $$  \ $$| $$$$$$$$| $$  \__/      | $$__  $$| $$  \ $$  | $$    
| $$    $$| $$  | $$| $$  | $$| $$_____/| $$            | $$  \ $$| $$  | $$  | $$ /$$
|  $$$$$$/|  $$$$$$$| $$$$$$$/|  $$$$$$$| $$            | $$$$$$$/|  $$$$$$/  |  $$$$/
 \______/  \____  $$|_______/  \_______/|__/            |_______/  \______/    \___/  
           /$$  | $$                                                                  
          |  $$$$$$/                                                                  
           \______/                                                                                                                                                   
";


            Random rand = new Random();
            // 1. Pick a random color (1-15 avoids black)
            Console.Write(AsciiDisplay.Text, Console.ForegroundColor = (ConsoleColor)rand.Next(1, 15));
            Thread.Sleep(100);

           
        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Rewind the video to the beginning and replay it
            BackgroundVideo.Position = TimeSpan.FromMilliseconds(1);
            BackgroundVideo.Play();

        }
    }
}
