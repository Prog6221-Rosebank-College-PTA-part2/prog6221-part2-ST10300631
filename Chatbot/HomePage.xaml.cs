using System;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Media;// Required for SoundPlayer
using System.Threading.Tasks;


namespace Chatbot
{ 
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            // Start the video loop automatically on startup
            BackgroundVideo.Play();

        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Rewind the video to the beginning and replay it
            BackgroundVideo.Position = TimeSpan.FromMilliseconds(1);
            BackgroundVideo.Play();
        }

        private async void Btnstr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Path to your external sound file (relative to the executable)
                SoundPlayer player = new SoundPlayer(@"Sounds\sonic_ring_sound_effect.wav");

                // Plays the sound asynchronously (does not freeze the UI thread)
                player.Play();
                await Task.Delay(4000);

                SoundPlayer Myvoice = new SoundPlayer(@"Sounds\My voice.wav");
                Myvoice.Play();
                MessageBox.Show("Hello welcome to cybersecurity awareness bot I'm here to help you stay safe online.");
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound: {ex.Message}");
            }

        }

    }
    
}
