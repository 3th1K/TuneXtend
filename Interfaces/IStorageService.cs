using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuneXtend.Models.Spotify;

namespace TuneXtend.Interfaces
{
    public interface IStorageService
    {
        public Task<bool> SetCodeVerifierAsync(string codeVarifier);
        public Task<string> GetCodeVerifierAsync();
        public Task<Token> GetSpotifyTokenAsync();
        public Task<bool> SetSpotifyTokenAsync(Token token);
        public Task<bool> ValidateSpotifyTokenAsync(Token token);
        public Task<string> GetSpotifyRefreshTokenAsync();

    }
}
