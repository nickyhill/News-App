using CyberNewsApp.ViewModel;

namespace CyberNewsApp.View.Mobile
{
    public partial class MobileMainPage : ContentPage
    {
        public MobileMainPage(NewsViewModel newsViewModel)
        {
            InitializeComponent();
            BindingContext = newsViewModel;
        }

    }

}
