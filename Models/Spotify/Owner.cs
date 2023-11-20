namespace TuneXtend.Models.Spotify;

public class Owner
{
    public ExternalUrls external_urls { get; set; }
    public Followers followers { get; set; }
    public string href { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
    public string display_name { get; set; }
}