using URLShortening.Application.ViewModels;

namespace URLShortening.Application.Interfaces
{
    public interface IUrlShorteningService
	{
        Task<ShortenUrlResponse> ShortenUrl(ShortenUrlRequest request);
        Task<RetrieveUrlResponse> RetrieveUrl(string shortUrl);
        Task<GetUrlHitsResponse> GetUrlHits(string shortUrl);
    }
}

