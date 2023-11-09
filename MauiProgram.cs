using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using TuneXtend.Interfaces;
using TuneXtend.Pages;
using TuneXtend.Services;

namespace TuneXtend
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IStorageService, StorageService>();
            builder.Services.AddSingleton<ISpotifyRestService, SpotifyRestService>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<CopyPlaylistSpotify>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}