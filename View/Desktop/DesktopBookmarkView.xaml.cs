using CyberNewsApp.ViewModel;


namespace CyberNewsApp.View.Desktop
{
	public partial class DesktopBookmarkView : ContentPage
	{
		public DesktopBookmarkView(BookmarkViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}

}