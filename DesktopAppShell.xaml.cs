using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using CyberNewsApp.View.Desktop;
using CyberNewsApp.ViewModel;

namespace CyberNewsApp
{
    public partial class DesktopAppShell : Shell
    {

        private readonly NewsViewModel _newsViewModel;
        private readonly BookmarkViewModel _bookmarkViewModel;

        public DesktopAppShell(NewsViewModel newsViewModel, BookmarkViewModel bookmarkViewModel)
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
                DesktopTitleBar.UpdateContext(
                    Current.CurrentPage.Title,
                    _newsViewModel,
                    _bookmarkViewModel
                );
            }
        }
    }
}
