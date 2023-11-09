using TuneXtend.Pages;

namespace TuneXtend
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(CopyPlaylistSpotify), typeof(CopyPlaylistSpotify));
            InitializeComponent();
        }
    }
}