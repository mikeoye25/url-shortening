namespace URLShortening.Application.AppSettings
{
    public class FixedOptions
    {
        public int QueueLimit { get; set; } = default!;
        public const string Fixed = "FixedOptions";
        public int PermitLimit { get; set; } = default!;
        public double Window { get; set; } = default!;
    }
}

