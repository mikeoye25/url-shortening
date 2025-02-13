using URLShortening.Domain.Entities;

namespace URLShortening.Application.Contracts
{
    public interface IShortenedUrlRepository : IAsyncRepository<ShortenedUrl>
    {
    }
}

