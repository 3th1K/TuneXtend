using TuneXtend.Models.Spotify;
namespace TuneXtend.Interfaces
{
    public interface ISpotifyRestService
    {
        Task<Token> GetTokenAsync(string authCode = null, bool refresh = false);

    }
}
