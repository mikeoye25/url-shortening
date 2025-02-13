using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortening.Domain.Entities
{
    [Table("shortenedurls")]
    public class ShortenedUrl : EntityBase
    {
        public string LongUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}

