using CyberNewsApp.ViewModel;
using System.Diagnostics;


namespace CyberNewsApp.View.Mobile
{
	public partial class MobileBookmarkView : ContentPage
	{
		public MobileBookmarkView(BookmarkViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}

        private void RemoveBookmark(object sender, EventArgs e)
        {
            var imageButton = (ImageButton)sender;
            var articleToDelete = imageButton.BindingContext as Model.NewsModel.Article;
            if (articleToDelete != null)
            {
                // Call your view model method to remove the bookmark
                var viewModel = (BookmarkViewModel)BindingContext;
                viewModel.RemoveBookmark(articleToDelete);
            }


        }

        private async void BookmarkTappedUrl(object sender, TappedEventArgs e)
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