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
            case "News Sort":
                BindingContext = newsViewModel;
                mobileTitleLabel.Text = "News Sort"; // MainPage title
                break;

            case "Bookmarks":
                BindingContext = bookmarkViewModel;
                mobileTitleLabel.Text = "Bookmarks"; // BookmarkPage title
                break;

            case "Help":
                BindingContext = bookmarkViewModel;
                mobileTitleLabel.Text = "Help and Support"; // BookmarkPage title
                break;

            default:
                mobileTitleLabel.Text = "CyberNewsApp"; // Fallback title
                break;
        }
    }
}
