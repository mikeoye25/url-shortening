using URLShortening.Application.Contracts;
using URLShortening.Domain.Entities;

namespace URLShortening.Infrastructure.Repositories
{
    public class ShortenedUrlRepository : RepositoryBase<ShortenedUrl>, IShortenedUrlRepository
    {
        public ShortenedUrlRepository(URLShorteningContext dbContext) : base(dbContext)
        {
        }
    }
}

