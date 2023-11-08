using System.Text.Json;
using TuneXtend.Interfaces;
using TuneXtend.Models.Spotify;

namespace TuneXtend.Services
{
    public class StorageService : IStorageService
    {
        public const string SPOTIFY_TOKEN_KEY = "SpotifyTokenKey";
        public async Task<Token> GetSpotifyTokenAsync()
        {
            var token = await SecureStorage.GetAsync(SPOTIFY_TOKEN_KEY);
            if(token is null)
                return null;
            try
            {
                var tokenObject = JsonSerializer.Deserialize<Token>(token);
                return tokenObject;
            }
            catch (Exception ex)
            {
                // do something
                return null;
            }
        }

        public async Task<bool> SetSpotifyTokenAsync(Token token)
        {
            throw new NotImplementedException();
        }
    }
}
