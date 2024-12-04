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
	}

}