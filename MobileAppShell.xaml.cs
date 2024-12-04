using CyberNewsApp.View.Mobile;
using CyberNewsApp.ViewModel;


namespace CyberNewsApp;

public partial class MobileAppShell : Shell
{
    private readonly NewsViewModel _newsViewModel;
    private readonly BookmarkViewModel _bookmarkViewModel;

    public MobileAppShell(NewsViewModel newsViewModel, BookmarkViewModel bookmarkViewModel)
    {
        InitializeComponent();

        // Store view models for later reference
        _newsViewModel = newsViewModel;
        _bookmarkViewModel = bookmarkViewModel;

        // Handle navigation updates
        Navigated += OnNavigated;
    }

    private void OnNavigated(object sender, ShellNavigatedEventArgs e)
    {
        if (Current?.CurrentPage?.Title != null)
        {
            // Update the title bar dynamically
            MobileTitleBar.UpdateContext(
                Current.CurrentPage.Title,
                _newsViewModel,
                _bookmarkViewModel
            );
        }
    }
}