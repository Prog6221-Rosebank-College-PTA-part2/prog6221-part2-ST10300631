using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;


namespace Chatbot
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        public Game()
        {
            InitializeComponent();
            MessageBox.Show("Coming soon");
            // Path to your external sound file (relative to the executable)
            SoundPlayer player = new SoundPlayer(@"Sounds\Constrution sound.wav");

            // Plays the sound asynchronously (does not freeze the UI thread)
            player.Play();

        }
       

    }
}
