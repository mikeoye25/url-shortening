namespace URLShortening.Application.ViewModels
{
    public class GetUrlHitsResponse
    {
        public int UrlHits { get; set; }
        public string? ErrorReason { get; set; }
        public bool Success { get; set; }
    }
}

