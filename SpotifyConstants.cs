using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TuneXtend
{
    public class SpotifyConstants
    {
        public const string REDIRECT_URI = "http://localhost:8080/callback";
        public const string AUTH_URI = "https://accounts.spotify.com/authorize";
        public const string TOKEN_URI = "https://accounts.spotify.com/api/token";
        public const string BASE_URI = "https://api.spotify.com/v1/";

        public const string CLIENT_ID = "";
        public const string CLIENT_SECRET = "";
        //public static readonly string STATE = Convert.ToBase64String(ConvertToSha256(GenerateRandomString(128)));
        public static readonly string CODE_VERIFIER = GenerateRandomString(128);
        public static readonly string CODE_CHALLANGE = Convert.ToBase64String(ConvertToSha256(CODE_VERIFIER));

        public const string SCOPE = "user-read-private " +
                                    "user-read-email " + 
                                    "ugc-image-upload " + 
                                    "playlist-read-private " + 
                                    "playlist-read-collaborative " + 
                                    "playlist-modify-private " + 
                                    "playlist-modify-public " + 
                                    "user-library-read " + 
                                    "user-library-modify ";

        public const string RESPONSE_TYPE = "code";

        private static byte [] ConvertToSha256(string s) => SHA256.HashData(Encoding.UTF8.GetBytes(s));

        private static string GenerateRandomString(int length)
        {
            Random rand = new Random();
            string charbase = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length)
                .Select(_ => charbase[rand.Next(charbase.Length)])
                .ToArray());
        }

    }
}
