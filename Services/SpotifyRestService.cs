using System.Diagnostics;
using System.Text.Json;
using System.Web;
using TuneXtend.Models.Spotify;
using TuneXtend.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;

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

        public async Task<UserData> GetCurrentUserProfileAsync()
        {
            string uri = "https://api.spotify.com/v1/me";

            var token = await _storageService.GetSpotifyTokenAsync();
            _restClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.access_token);
            try
            {
                var response = await _restClient.GetAsync(uri);
                var responseStream = await response.Content.ReadAsStreamAsync();
                if (response.IsSuccessStatusCode)
                {
                    var userData =
                        JsonSerializer.Deserialize<UserData>(responseStream);
                    return userData;
                }
            }
            catch (Exception ex)
            {
                // todo: handle get request exceptions
            }
            return null;
        }

        public async Task<PlaylistItem> GetPlaylistAsync(string playlistId)
        {
            string uri = $"{SpotifyConstants.BASE_URI}/playlists/{playlistId}";

            var token = await _storageService.GetSpotifyTokenAsync();
            _restClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.access_token);
            try
            {
                var response = await _restClient.GetAsync(uri);
                var responseStream = await response.Content.ReadAsStreamAsync();
                if (response.IsSuccessStatusCode)
                {
                    var userPlaylistData =
                        JsonSerializer.Deserialize<PlaylistItem>(responseStream);
                    return userPlaylistData;
                }
            }
            catch (Exception ex)
            {
                // todo: handle get request exceptions
            }
            return null;
        }

        public async Task<PlaylistItem> CreatePlaylistAsync(string playlistName, string playlistDescription = "", bool isPublic = false, string userId = null)
        {
            if (userId is null)
            {
                var currentUserData = await GetCurrentUserProfileAsync();
                userId = currentUserData.id;
            }
            string uri = $"{SpotifyConstants.BASE_URI}/users/{userId}/playlists";

            var json = '{' +
                       $"\"name\":\"{playlistName}\"," +
                       $"\"description\":\"{playlistDescription}\"," +
                       $"\"public\":\"{isPublic}\"" +
                       '}';
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var token = await _storageService.GetSpotifyTokenAsync();
            _restClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.access_token);
            try
            {
                var response = await _restClient.PostAsync(uri, content);
                var responseStream = await response.Content.ReadAsStreamAsync();
                if (response.IsSuccessStatusCode)
                {
                    var newPlaylist =
                        JsonSerializer.Deserialize<PlaylistItem>(responseStream);
                    return newPlaylist;
                }
            }
            catch (Exception ex)
            {
                // todo: handle get request exceptions
            }
            return null;
        }

        public async Task<List<Track>> GetPlaylistTracksAsync(string playlistId)
        {
            string uri = $"{SpotifyConstants.BASE_URI}/playlists/{playlistId}/tracks";

            var token = await _storageService.GetSpotifyTokenAsync();
            _restClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.access_token);

            List<Track> tracks = new List<Track>();
            var playlistData = new PlaylistData();
            playlistData.next = uri;
            try
            {
                while (playlistData.next is not null)
                {
                    var response = await _restClient.GetAsync(playlistData.next);
                    var responseData = await response.Content.ReadAsStringAsync();
                    playlistData.next = null;
                    playlistData = JsonSerializer.Deserialize<PlaylistData>(responseData);
                    var playlistItems = playlistData.items;
                    foreach (var item in playlistItems)
                    {
                        var track = item.track;
                        tracks.Add(track);
                    }
                }
                return tracks;
            }
            catch (Exception ex)
            {
                // todo: handle get request exceptions
            }
            return null;
        }

        public async Task<string> AddTrackToPlaylist(string playlistId, string songId)
        {
            string uri = $"{SpotifyConstants.BASE_URI}/playlists/{playlistId}/tracks";
            var json = '{' + "\"uris\":[" + $"\"spotify:track:{songId}\"]" + '}';
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var token = await _storageService.GetSpotifyTokenAsync();
            _restClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.access_token);
            try
            {
                var response = await _restClient.PostAsync(uri, content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return responseString;
                }

                return null;
            }
            catch (Exception ex)
            {
                // todo: handle get request exceptions
            }
            return null;
        }

        public async Task<List<PlaylistItem>> GetUserPlaylistsAsync(string userId = null)
        {
            string uri;
            if (userId is null)
            {
                uri = $"{SpotifyConstants.BASE_URI}/me/playlists";
            }
            else
            {
                uri = $"{SpotifyConstants.BASE_URI}/users/{userId}/playlists";
            }

            var token = await _storageService.GetSpotifyTokenAsync();
            _restClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.access_token);
            List<PlaylistItem> playlistItems = new List<PlaylistItem>();
            var userPlaylistResponse = new UserPlaylistsResponse();
            userPlaylistResponse.next = uri;
            try
            {
                while (userPlaylistResponse.next is not null)
                {
                    var response = await _restClient.GetAsync(userPlaylistResponse.next);
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    userPlaylistResponse.next = null;
                    if (response.IsSuccessStatusCode)
                    {
                        userPlaylistResponse = JsonSerializer.Deserialize<UserPlaylistsResponse>(responseStream);
                        foreach (var playlistItem in userPlaylistResponse.items)
                        {
                            if (playlistItem.images.Count < 1)
                            {
                                playlistItem.images.Add(new()
                                {
                                    url = "https://i.scdn.co/image/ab67616d00001e02ff9ca10b55ce82ae553c8228",
                                    height = 300,
                                    width = 300
                                });
                            }
                            playlistItems.Add(playlistItem);
                        }
                    }
                }
                return playlistItems;

            }
            catch (Exception ex)
            {
                // todo: handle get request exceptions
            }
            return null;
        }
    }
}
