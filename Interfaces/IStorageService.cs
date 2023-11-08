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
        public Task<Token> GetSpotifyTokenAsync();
        public Task<bool> SetSpotifyTokenAsync(Token token);

    }
}
