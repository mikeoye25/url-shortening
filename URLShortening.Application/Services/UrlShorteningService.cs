using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using URLShortening.Application.Contracts;
using URLShortening.Application.Interfaces;
using URLShortening.Application.ViewModels;
using URLShortening.Domain;
using URLShortening.Domain.Constants;
using URLShortening.Domain.Entities;

namespace URLShortening.Application.Services
{
    public class UrlShorteningService : IUrlShorteningService
    {
        private readonly IShortenedUrlRepository ShortenedUrlRepository;
        private readonly ILogger<UrlShorteningService> Logger;
        private readonly Random _random = new();
        private IMemoryCache Cache;
        private const string codeCacheKey = "code";

        public UrlShorteningService(IShortenedUrlRepository shortenedUrlRepository, ILogger<UrlShorteningService> logger, IMemoryCache cache)
        {
            ShortenedUrlRepository = shortenedUrlRepository ?? throw new ArgumentNullException(nameof(shortenedUrlRepository));
            Logger = logger;
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

        private static string GetCode(string url)
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

        private async Task<RetrieveUrlResponse> UpdateUrl(string code)
        {
            var response = new RetrieveUrlResponse { Success = false };
            var shortenedUrl = await ShortenedUrlRepository.GetFirstOrDefault(s => s.Code == code);
            if (shortenedUrl is null)
            {
                response.ErrorReason = ErrorConstants.SHORTENED_URL_DOES_NOT_EXIST;
                return response;
            }
            shortenedUrl.HitCount++;
            var data = await ShortenedUrlRepository.Update(shortenedUrl);
            if (data is null)
            {
                response.ErrorReason = ErrorConstants.SHORTENED_URL_UPDATE_FAILED;
                return response;
            }

            Cache.Remove($"{codeCacheKey}:{code}");
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
            Cache.Set($"{codeCacheKey}:{code}", data, cacheEntryOptions);

            response.LongUrl = shortenedUrl.LongUrl;
            response.Success = true;
            return response;
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
                HitCount = 0,
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
            var code = GetCode(shortUrl);

            // In-Memory Caching
            if (Cache.TryGetValue($"{codeCacheKey}:{code}", out ShortenedUrl? url))
            {
                Logger.LogInformation($"Code: {code} found in cache.");
                response.LongUrl = url?.LongUrl;
                await UpdateUrl(code);
            }
            else
            {
                Logger.LogInformation("Code not found in cache. Fetching from database.");
                var updatedUrl = await UpdateUrl(code);
                response.LongUrl = updatedUrl.LongUrl;
            }

            response.Success = true;
            return response;
        }


        public async Task<GetUrlHitsResponse> GetUrlHits(string shortUrl)
        {
            Logger.LogInformation("Executing {Action} with parameters: {Parameters}", nameof(GetUrlHits), shortUrl);
            var response = new GetUrlHitsResponse { Success = false };
            var code = GetCode(shortUrl);
            var shortenedUrl = await ShortenedUrlRepository.GetFirstOrDefault(s => s.Code == code);
            if (shortenedUrl is null)
            {
                response.ErrorReason = ErrorConstants.SHORTENED_URL_DOES_NOT_EXIST;
                return response;
            }
            response.UrlHits = shortenedUrl.HitCount;
            response.Success = true;
            return response;
        }
    }
}

