namespace GoodNewsAggregator.Data.Entities
{
    public class News : IBaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullText { get; set; }
        public DateTime PublicationDate { get; set; }    
        public double Rate { get; set; }
        public List<Comment> Comments { get; set; }
        public int SourceId { get; set; }
        public Source Source { get; set; }
    }
}
