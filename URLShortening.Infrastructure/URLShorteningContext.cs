using Microsoft.EntityFrameworkCore;
using URLShortening.Domain.Constants;
using URLShortening.Domain.Entities;

namespace URLShortening.Infrastructure
{
    public class URLShorteningContext : DbContext
    {
        public URLShorteningContext(DbContextOptions<URLShorteningContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder
                    .Property(shortenedUrl => shortenedUrl.Code)
                    .HasMaxLength(ShortLinkSettings.Length);

                builder
                    .HasIndex(shortenedUrl => shortenedUrl.Code)
                    .IsUnique();
            });
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

