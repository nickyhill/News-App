using CyberNewsApp.ViewModel;


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
    }

}