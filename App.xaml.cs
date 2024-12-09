namespace CyberNewsApp
{
    public partial class App : Application
    {
        public App(DesktopAppShell desktopAppShell, MobileAppShell mobileAppShell)
        {
            InitializeComponent();

            UserAppTheme = AppTheme.Light;

#if ANDROID || IOS
            MainPage = mobileAppShell;
#else
            MainPage = desktopAppShell;
#endif
        }
    }
}
