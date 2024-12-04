using CyberNewsApp.Model;
using CyberNewsApp.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CyberNewsApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            // Register dependencies
            builder.Services.AddSingleton<NewsModel>();
            builder.Services.AddSingleton<NewsViewModel>();
            builder.Services.AddSingleton<View.Desktop.DesktopMainPage>();
            builder.Services.AddSingleton<View.Mobile.MobileMainPage>();
            builder.Services.AddSingleton<BookmarkModel>();
            builder.Services.AddSingleton<BookmarkViewModel>();
            builder.Services.AddTransient<View.Desktop.DesktopBookmarkView>();
            builder.Services.AddSingleton<View.Mobile.MobileBookmarkView>();
            builder.Services.AddSingleton<View.Desktop.DesktopTitleBarView>();
            builder.Services.AddSingleton<View.Mobile.MobileTitleBarView>();

            builder.Services.AddSingleton<DesktopAppShell>();
            builder.Services.AddSingleton<MobileAppShell>();

            IServiceCollection services = builder.Services;

            // Set up Console Logging
            services.AddSerilog(
                new LoggerConfiguration()
                .WriteTo.Debug()
                .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger());

            // Add Serilog to .NET ILogger pipeline
            builder.Logging.ClearProviders(); // Remove default providers
            builder.Logging.AddSerilog(Log.Logger); // Add Serilog


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
