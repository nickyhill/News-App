using CyberNewsApp.ViewModel;

namespace CyberNewsApp.View.Mobile;

public partial class MobileTitleBarView : ContentView
{
    public MobileTitleBarView()
    {
        InitializeComponent();
    }

    public void UpdateContext(string currentPageTitle, NewsViewModel newsViewModel, BookmarkViewModel bookmarkViewModel)
    {
        switch (currentPageTitle)
        {
            case "NewsSort":
                BindingContext = newsViewModel;
                mobileTitleLabel.Text = "NewsSort"; // MainPage title
                break;

            case "Bookmarks":
                BindingContext = bookmarkViewModel;
                mobileTitleLabel.Text = "Bookmarks"; // BookmarkPage title
                break;

            default:
                mobileTitleLabel.Text = "CyberNewsApp"; // Fallback title
                break;
        }
    }
}
