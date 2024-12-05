using CyberNewsApp.ViewModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CyberNewsApp.View.Mobile
{
    public partial class MobileMainPage : ContentPage
    {
        public MobileMainPage(NewsViewModel newsViewModel)
        {
            InitializeComponent();
            BindingContext = newsViewModel;
        }

        private async void MainTappedUrl(object sender, TappedEventArgs e)
        {
            if (sender is Label label)
            {
                string url = label.Text; // Get the URL from the Label's Text
                if (!string.IsNullOrWhiteSpace(url))
                {
                    Debug.WriteLine($"Opening URL: {url}");
                    await Launcher.OpenAsync(url); // Open the URL in the browser
                }
                else
                {
                    Debug.WriteLine("URL is empty or null.");
                }
            }
            else
            {
                Debug.WriteLine("Sender is not a Label.");
            }
        }
    }

}
