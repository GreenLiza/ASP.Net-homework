namespace GoodNewsAggregator.Data.Entities
{
    public class Comment : IBaseEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime PublicationDate { get; set; }
        public int NewsId { get; set; }
        public News News { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
