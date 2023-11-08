using Mopups.Services;
using TuneXtend.Interfaces;
using TuneXtend.Pages;

namespace TuneXtend
{
    public partial class MainPage : ContentPage
    {
        private readonly IStorageService _storageService;
        public MainPage(IStorageService storageService)
        {
            _storageService = storageService;
            InitializeComponent();
        }

        private async void BtnCopySpotifyPlaylist_OnClicked(object sender, EventArgs e)
        {
            BtnCopySpotifyPlaylist.IsEnabled = false;
            var spotifyToken = await _storageService.GetSpotifyTokenAsync();
            if (spotifyToken is null)
            {
                var x = new UserLoginPopupPage("https://google.com");
                await MopupService.Instance.PushAsync(x);
                //Task.Run(async () => { await MopupService.Instance.PushAsync(x); }).Wait();
                var rvalue = await x.PopupDismissedTask;
            }
            BtnCopySpotifyPlaylist.IsEnabled = true;
        }
    }
}