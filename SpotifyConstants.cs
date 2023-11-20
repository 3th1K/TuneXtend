using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TuneXtend
{
    public class SpotifyConstants
    {
        public const string REDIRECT_URI = "https://localhost:8888/callback";
        public const string AUTH_URI = "https://accounts.spotify.com/authorize";
        public const string TOKEN_URI = "https://accounts.spotify.com/api/token";
        public const string BASE_URI = "https://api.spotify.com/v1";

        public const string CLIENT_ID = "dc8f8cc260fa472b880517380d215459";
        public const string CLIENT_SECRET = "";
        public const string CODE_CHALLANGE_METHOD = "S256";
        public const string RESPONSE_TYPE = "code";

        public static readonly string CODE_VERIFIER = GenerateCodeVerifier();
        public static readonly string CODE_CHALLANGE = GenerateCodeChallange(CODE_VERIFIER);

        public const string SCOPE = "user-read-private " +
                                    "user-read-email " + 
                                    "ugc-image-upload " + 
                                    "playlist-read-private " + 
                                    "playlist-read-collaborative " + 
                                    "playlist-modify-private " + 
                                    "playlist-modify-public " + 
                                    "user-library-read " + 
                                    "user-library-modify ";


        private static string GenerateCodeChallange(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }

        private static string GenerateCodeVerifier()
        {
            //Random rand = new Random();
            //string charbase = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //return new string(Enumerable.Range(0, length)
            //    .Select(_ => charbase[rand.Next(charbase.Length)])
            //    .ToArray());
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var random = new Random();
            var nonce = new char[128];
            for (int i = 0; i < nonce.Length; i++)
            {
                nonce[i] = chars[random.Next(chars.Length)];
            }

            return new string(nonce);
        }

    }
}
