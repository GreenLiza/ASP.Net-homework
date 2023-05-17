namespace GoodNewsAggregator.Data.Entities
{
    public class Source : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RssLink { get; set; }
        public string Link { get; set; }
        public List<News> News { get; set; }
    }
}
