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
                var popupPage = new UserLoginPopupPage("https://google.com");
                await MopupService.Instance.PushAsync(popupPage);
                var popupResult = await popupPage.PopupDismissedTask;
                if (popupResult is null)
                {

                }
            }
            BtnCopySpotifyPlaylist.IsEnabled = true;
        }
    }
}