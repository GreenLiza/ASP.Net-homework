namespace GoodNewsAggregator.Data.Entities
{
    public class RatingWord : IBaseEntity
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public double Rate { get; set; }
    }
}
