using CyberNewsApp.ViewModel;

namespace CyberNewsApp.View.Desktop;

public partial class DesktopTitleBarView : ContentView
{
    public DesktopTitleBarView()
    {
        InitializeComponent();
    }

    public void UpdateContext(string currentPageTitle, NewsViewModel newsViewModel, BookmarkViewModel bookmarkViewModel)
    {
        switch (currentPageTitle)
        {
            case "NewsSort":
                BindingContext = newsViewModel;
                desktopTitleLabel.Text = "NewsSort"; // MainPage title
                break;

            case "Bookmarks":
                BindingContext = bookmarkViewModel;
                desktopTitleLabel.Text = "Bookmarks"; // BookmarkPage title
                break;

            default:
                desktopTitleLabel.Text = "CyberNewsApp"; // Fallback title
                break;
        }
    }
}
