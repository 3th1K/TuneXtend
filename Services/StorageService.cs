using System.Text.Json;
using TuneXtend.Interfaces;
using TuneXtend.Models.Spotify;

namespace TuneXtend.Services
{
    public class StorageService : IStorageService
    {
        public const string SPOTIFY_TOKEN_KEY = "SpotifyTokenKey";
        public const string SPOTIFY_CODE_VERIFIER_KEY = "SpotifyCodeVerifierKey";
        public async Task<bool> SetCodeVerifierAsync(string codeVarifier)
        {
            try
            {
                await SecureStorage.SetAsync(SPOTIFY_CODE_VERIFIER_KEY, codeVarifier);
                return true;
            }
            catch (Exception ex)
            {
                //todo : add code to handle spotify set code verifier exception
                return false;
            }
        }

        public async Task<string> GetCodeVerifierAsync()
        {
            var code = await SecureStorage.GetAsync(SPOTIFY_CODE_VERIFIER_KEY);
            return code;
        }

        public async Task<bool> ValidateSpotifyTokenAsync(Token token)
        {
            if (token.expiry is null || DateTime.Now > token.expiry)
            {
                return false;
            }
            return true;
        }

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
                //todo : add code to handle get spotify token exception
                return null;
            }
        }

        public async Task<bool> SetSpotifyTokenAsync(Token token)
        {
            try
            {
                var serializedToken = JsonSerializer.Serialize(token, typeof(Token));
                await SecureStorage.SetAsync(SPOTIFY_TOKEN_KEY, serializedToken);
                return true;
            }
            catch (Exception ex)
            {
                // todo : better handling
                return false;
            }
        }

        public async Task<string> GetSpotifyRefreshTokenAsync()
        {
            var token = await SecureStorage.GetAsync(SPOTIFY_TOKEN_KEY);
            if (token is null)
                return null;
            try
            {
                var tokenObject = JsonSerializer.Deserialize<Token>(token);
                return tokenObject.refresh_token;
            }
            catch (Exception ex)
            {
                //todo : add code to handle get spotify token exception
                return null;
            }
        }
    }
}
