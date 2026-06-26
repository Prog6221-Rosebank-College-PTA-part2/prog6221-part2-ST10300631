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
        public class Question
        {
            public string questionText { get; set; } = "";
            public string[] Options { get; set; } = new string[4];
            public int correctAnswer { get; set; }
        }
        int questionNumber = 1;
        int score = 0;
        Dictionary<int, Question> questions = new Dictionary<int, Question>();

        public  Game()
        {
            InitializeComponent();
            questions.Add(1, new Question
            {
                questionText = "c# is a programmming language",
                Options = new string[2] { "true", "false" },
                correctAnswer = 1
            });
            questions.Add(2, new Question
            {
                questionText = "which control is used for selecting one option",
                Options = new string[4] { "Textbox", "RadioButton", "Button", "Label" },
                correctAnswer = 2
            });
            questions.Add(3, new Question
            {
                questionText = "WPF stands for windows presentation foundation",
                Options = new string[2] { "true", "false" },
                correctAnswer = 1
            });
            questions.Add(4, new Question
            {
                questionText = "which data type is used for true or false?",
                Options = new string[4] { "int", "string", "bool", "double" },
                correctAnswer = 3
            });
            LoadQuestion();

            // Start the video loop automatically on startup
            BackgroundVideo.Play();
        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Rewind the video to the beginning and replay it
            BackgroundVideo.Position = TimeSpan.FromMilliseconds(1);
            BackgroundVideo.Play();
        }

            private void LoadQuestion()
        {
            Question correctQuestion = questions[questionNumber];
            txtQuestion.Text = correctQuestion.questionText;
            option1.Content = correctQuestion.Options[0];
            option2.Content = correctQuestion.Options[1];
            option1.Visibility = Visibility.Visible;
            option2.Visibility = Visibility.Visible;
            option3.Visibility = Visibility.Hidden;
            option4.Visibility = Visibility.Hidden;
            if (correctQuestion.Options.Length == 4)
            {
                option3.Content = correctQuestion.Options[2];
                option4.Content = correctQuestion.Options[3];
                option3.Visibility = Visibility.Visible;
                option4.Visibility = Visibility.Visible;
            }
            option1.IsChecked = false;
            option2.IsChecked = false;
            option3.IsChecked = false;
            option4.IsChecked = false;
            txtScore.Text = "Score:" + score;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int selectedAnswer = 0;
            if (option1.IsChecked == true)
            {
                selectedAnswer = 1;
            }
            else if (option2.IsChecked == true)
            {
                selectedAnswer = 2;
            }
            else if (option3.IsChecked == true)
            {
                selectedAnswer = 3;
            }
            else if (option3.IsChecked == true)
            {
                selectedAnswer = 4;
            }
            else
            {
                MessageBox.Show("please select an answer"); return;
            }
            Question correctQuestion = questions[questionNumber];
            if (selectedAnswer == correctQuestion.correctAnswer)
            {
                score++;
                MessageBox.Show("correct answer");
            }
            else
            {
                MessageBox.Show("wrong answer");
            }
            txtScore.Text = "Score :" + score;
            questionNumber++;
            if (questionNumber <= questions.Count)
            {
                LoadQuestion();
            }
            else
            {
                MessageBox.Show("ur quiz is finished" + score + "out of " + questions.Count);
                btnSubmit.IsEnabled = false;
            }
        }
    }
}