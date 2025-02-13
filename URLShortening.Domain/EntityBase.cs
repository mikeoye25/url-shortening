namespace URLShortening.Domain
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

