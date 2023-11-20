using System.Net;
using System.Web;
using Mopups.Services;
using TuneXtend.Interfaces;
using TuneXtend.Models.Spotify;
using TuneXtend.Pages;

namespace TuneXtend
{
    public partial class MainPage : ContentPage
    {
        private readonly IStorageService _storageService;
        private readonly ISpotifyRestService _spotifyRestService;
        public MainPage(IStorageService storageService, ISpotifyRestService spotifyRestService)
        {
            _storageService = storageService;
            _spotifyRestService = spotifyRestService;
            InitializeComponent();
        }

        private async void BtnCopySpotifyPlaylist_OnClicked(object sender, EventArgs e)
        {
            BtnCopySpotifyPlaylist.IsEnabled = false;
            var spotifyToken = await _storageService.GetSpotifyTokenAsync();
            if (spotifyToken is null)
            {
                bool setCodeVerifier = await _storageService.SetCodeVerifierAsync(SpotifyConstants.CODE_VERIFIER);
                if (!setCodeVerifier)
                {
                    // todo : handle if setting code verifier is not success
                    return;
                }

                string queryString = $"{SpotifyConstants.AUTH_URI}?" +
                                     $"response_type={SpotifyConstants.RESPONSE_TYPE}" +
                                     $"&client_id={SpotifyConstants.CLIENT_ID}" +
                                     $"&scope={WebUtility.UrlEncode(SpotifyConstants.SCOPE)}" +
                                     $"&code_challenge_method={SpotifyConstants.CODE_CHALLANGE_METHOD}" +
                                     $"&code_challenge={SpotifyConstants.CODE_CHALLANGE}" +
                                     $"&redirect_uri={WebUtility.UrlEncode(SpotifyConstants.REDIRECT_URI)}";
                //$"&state={SpotifyConstants.SCOPE}";

                var popupPage = new UserLoginPopupPage(queryString);
                await MopupService.Instance.PushAsync(popupPage);
                var popupResult = await popupPage.PopupDismissedTask;
                if (popupResult is null)
                {
                    await DisplayAlert("Alert", "You need to be logged in to access this feature", "Ok");
                }
                else
                {
                    var callbackUrl = new Uri(popupResult);
                    var authCode = HttpUtility.ParseQueryString(callbackUrl.Query).Get("code");
                    if (authCode is null)
                    {
                        await DisplayAlert("Alert", "You need to be logged in to access this feature", "Ok");
                    }
                    else
                    {
                        Token token = await _spotifyRestService.GetTokenAsync(authCode);
                        if (token is null)
                        {
                            // todo : better error
                            await DisplayAlert("Error", "Something Went Wrong", "Ok");
                        }
                        else
                        {
                            var setToken = await _storageService.SetSpotifyTokenAsync(token);
                            if (setToken)
                            {
                                await Shell.Current.GoToAsync(nameof(CopyPlaylistSpotify));
                                return;
                            }
                            else
                            {
                                // todo : better error
                                await DisplayAlert("Error", "Something Went Wrong", "Ok");
                            }
                        }
                    }

                }
            }
            else
            {
                if (!await _storageService.ValidateSpotifyTokenAsync(spotifyToken))
                {
                    var token = await _spotifyRestService.GetTokenAsync(refresh: true);
                    await _storageService.SetSpotifyTokenAsync(token);
                }

                await Shell.Current.GoToAsync(nameof(CopyPlaylistSpotify));
            }

            BtnCopySpotifyPlaylist.IsEnabled = true;
        }
    }
}