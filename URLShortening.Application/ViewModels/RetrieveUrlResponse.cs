namespace URLShortening.Application.ViewModels
{
    public class RetrieveUrlResponse
    {
        public string? LongUrl { get; set; }
        public string? ErrorReason { get; set; }
        public bool Success { get; set; }
    }
}

