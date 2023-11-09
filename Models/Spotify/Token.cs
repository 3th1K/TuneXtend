namespace TuneXtend.Models.Spotify
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public DateTime? expiry { get; set; } = null;
        public string refresh_token { get; set; }

    }
}
