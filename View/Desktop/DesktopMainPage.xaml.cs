using CyberNewsApp.ViewModel;

namespace CyberNewsApp.View.Desktop
{
    public partial class DesktopMainPage : ContentPage
    {
        public DesktopMainPage(NewsViewModel newsViewModel)
        {
            InitializeComponent();
            BindingContext = newsViewModel;
        }

    }

}
