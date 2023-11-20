using TuneXtend.Models.Spotify;
namespace TuneXtend.Interfaces
{
    public interface ISpotifyRestService
    {
        Task<Token> GetTokenAsync(string authCode = null, bool refresh = false);
        Task<UserData> GetCurrentUserProfileAsync();
        Task<List<PlaylistItem>> GetUserPlaylistsAsync(string userId = null);
        Task<PlaylistItem> GetPlaylistAsync(string playlistId);
        Task<PlaylistItem> CreatePlaylistAsync(string playlistName, string playlistDescription = "", bool isPublic = false, string userId = null);
        Task<List<Track>> GetPlaylistTracksAsync(string playlistId);
        Task<string> AddTrackToPlaylist(string playlistId, string songId);

    }
}
