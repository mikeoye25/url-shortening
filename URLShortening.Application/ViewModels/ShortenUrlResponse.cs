namespace URLShortening.Application.ViewModels
{
    public class ShortenUrlResponse
    {
        public string? ShortUrl { get; set; }
        public string? ErrorReason { get; set; }
        public bool Success { get; set; }
    }
}

