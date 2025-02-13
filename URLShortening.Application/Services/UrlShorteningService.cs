using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging;
using URLShortening.Application.Contracts;
using URLShortening.Application.Interfaces;
using URLShortening.Application.ViewModels;
using URLShortening.Domain.Constants;
using URLShortening.Domain.Entities;

namespace URLShortening.Application.Services
{
    public class UrlShorteningService : IUrlShorteningService
    {
        private readonly IShortenedUrlRepository ShortenedUrlRepository;
        private readonly ILogger<UrlShorteningService> Logger;
        private readonly Random _random = new();

        public UrlShorteningService(IShortenedUrlRepository shortenedUrlRepository, ILogger<UrlShorteningService> logger)
        {
            ShortenedUrlRepository = shortenedUrlRepository ?? throw new ArgumentNullException(nameof(shortenedUrlRepository));
            Logger = logger;
        }

        private async Task<string> GenerateUniqueCode()
        {
            var codeChars = new char[ShortLinkSettings.Length];
            var maxValue = ShortLinkSettings.Alphabet.Length;

            while (true)
            {
                for (var i = 0; i < ShortLinkSettings.Length; i++)
                {
                    var randomIndex = _random.Next(maxValue);

                    codeChars[i] = ShortLinkSettings.Alphabet[randomIndex];
                }

                var code = new string(codeChars);
                // handles duplicate short URLs
                if (!await ShortenedUrlRepository.Any(s => s.Code == code))
                {
                    return code;
                }
            }
        }

        public async Task<ShortenUrlResponse> ShortenUrl(ShortenUrlRequest request)
        {
            Logger.LogInformation("Executing {Action} with parameters: {Parameters}", nameof(ShortenUrl), JsonSerializer.Serialize(request));
            var response = new ShortenUrlResponse { Success = false };
            Uri? createdUri = null;

            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out createdUri))
            {
                response.ErrorReason = ErrorConstants.INVALID_URL;
                return response;
            }

            var code = await GenerateUniqueCode();
            var shortenedUrl = new ShortenedUrl
            {
                LongUrl = request.Url,
                Code = code,
                ShortUrl = $"{createdUri.Scheme}://{code}",
                CreatedDate = DateTime.UtcNow
            };

            var data = await ShortenedUrlRepository.Insert(shortenedUrl);
            if (data is null)
            {
                response.ErrorReason = ErrorConstants.INSERT_SHORTENED_URL_FAILED;
                return response;
            }
            response.ShortUrl = shortenedUrl.ShortUrl;
            response.Success = true;
            return response;
        }

        public async Task<RetrieveUrlResponse> RetrieveUrl(string shortUrl)
        {
            Logger.LogInformation("Executing {Action} with parameters: {Parameters}", nameof(ShortenUrl), shortUrl);
            var response = new RetrieveUrlResponse { Success = false };
            shortUrl = HttpUtility.UrlDecode(shortUrl);
            Uri? createdUri = null;
            if (!Uri.TryCreate(shortUrl, UriKind.Absolute, out createdUri))
            {
                response.ErrorReason = ErrorConstants.INVALID_URL;
                return response;
            }
            var urlParts = shortUrl.Split('/');
            Logger.LogInformation($"urlParts: {urlParts.Length}");
            var code = string.Empty;
            if (urlParts.Length > 0)
            {
                code = urlParts[urlParts.Length - 1];
            }
            else
            {
                response.ErrorReason = ErrorConstants.INVALID_URL;
                return response;
            }
            var shortenedUrl = await ShortenedUrlRepository.GetFirstOrDefault(s => s.Code == code && s.ShortUrl == shortUrl);
            if (shortenedUrl is null)
            {
                response.ErrorReason = ErrorConstants.SHORTENED_URL_DOES_NOT_EXIST;
                return response;
            }
            response.LongUrl = shortenedUrl.LongUrl;
            response.Success = true;
            return response;
        }
    }
}

