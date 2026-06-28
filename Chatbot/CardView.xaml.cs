using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for CardView.xaml
    /// </summary>
    public partial class CardView : UserControl
    {


        // Dependency Properties for easier data binding and customization
        public static readonly DependencyProperty CardTitleProperty =
            DependencyProperty.Register("CardTitle", typeof(string), typeof(CardView), new PropertyMetadata("Card 1"));

        public string CardTitle
        {
            get { return (string)GetValue(CardTitleProperty); }
            set { SetValue(CardTitleProperty, value); }
        }

        public static readonly DependencyProperty CardDescriptionProperty =
            DependencyProperty.Register("CardDescription", typeof(string), typeof(CardView), new PropertyMetadata("Default description."));

        public string CardDescription
        {
            get { return (string)GetValue(CardDescriptionProperty); }
            set { SetValue(CardDescriptionProperty, value); }
        }

        public static readonly DependencyProperty CardImageSourceProperty =
            DependencyProperty.Register("CardImageSource", typeof(string), typeof(CardView), new PropertyMetadata("https://via.placeholder.com/150"));

        public string CardImageSource
        {
            get { return (string)GetValue(CardImageSourceProperty); }
            set { SetValue(CardImageSourceProperty, value); }
        }


        public CardView()
        {
            InitializeComponent();

            // Bind TextBlocks to Dependency Properties
            TitleTextBlock.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("CardTitle") { Source = this });
            DescriptionTextBlock.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("CardDescription") { Source = this });
            // For Image Source, you might need a converter or set it directly in XAML if it's a simple string URL
            // Or set it here if you prefer
            // ((Image)((Grid)((Button)this.Content).Content).Children[0]).Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(CardImageSource));
            ImgIcon.SetBinding(Image.SourceProperty, new System.Windows.Data.Binding("CardImageSource") { Source = this });

        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            // Handle the click event
            MessageBox.Show($"Card '{CardTitle}' Clicked!");
            e.Handled = true; // Prevent event from bubbling up if not desired
        }             
    
    }     
}



