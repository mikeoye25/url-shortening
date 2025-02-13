using System.Web;
using URLShortening.Domain.Constants;

namespace URLShortening.Application
{
    public static class Utilities
    {
        private static readonly Random Rnd = new();

        public static string GenerateCode()
        {
            var codeChars = new char[ShortLinkSettings.Length];
            var maxValue = ShortLinkSettings.Alphabet.Length;
            for (var i = 0; i < ShortLinkSettings.Length; i++)
            {
                var randomIndex = Rnd.Next(maxValue);
                codeChars[i] = ShortLinkSettings.Alphabet[randomIndex];
            }
            var code = new string(codeChars);
            return code;
        }

        public static string GetCode(string url)
        {
            url = HttpUtility.UrlDecode(url);
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                return string.Empty;
            }
            var urlParts = url.Split('/');
            if (urlParts.Length < 1)
            {
                return string.Empty;
            }
            return urlParts[urlParts.Length - 1];
        }
    }
}
