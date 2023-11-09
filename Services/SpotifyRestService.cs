using System.Diagnostics;
using System.Text.Json;
using System.Web;
using TuneXtend.Models.Spotify;
using TuneXtend.Interfaces;
using System.Net;

namespace TuneXtend.Services
{
    public class SpotifyRestService : ISpotifyRestService
    {
        private readonly HttpClient _restClient;
        private readonly IStorageService _storageService;
        public SpotifyRestService(HttpClient restClient, IStorageService storageService)
        {
            _restClient = restClient;
            _storageService = storageService;
        }
        public async Task<Token> GetTokenAsync(string authCode, bool refresh)
        {
            var codeVerifier = await _storageService.GetCodeVerifierAsync();
            FormUrlEncodedContent content;
            if (refresh)
            {
                var refreshToken = await _storageService.GetSpotifyRefreshTokenAsync();
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", SpotifyConstants.CLIENT_ID),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                });
            }
            else
            {
                content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", SpotifyConstants.CLIENT_ID),
                    new KeyValuePair<string, string>("code", authCode),
                    new KeyValuePair<string, string>("redirect_uri", SpotifyConstants.REDIRECT_URI),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code_verifier", codeVerifier)
                });
            }

            try
            {
                var response = await _restClient.PostAsync(SpotifyConstants.TOKEN_URI , content);
                if (response.IsSuccessStatusCode)
                {
                    var token = await JsonSerializer.DeserializeAsync<Token>(
                        await response.Content.ReadAsStreamAsync());
                    token.expiry = DateTime.Now.AddSeconds(token.expires_in);
                    return token;
                }

                return null;
            }
            catch (Exception ex)
            {
                //todo : handle exception
                return null;
            }
        }
    }
}
