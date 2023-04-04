using GoodNewsAggregator.Data.Entities;

namespace GoodNewsAggregator.MVC.Models
{
    public class NewsModel
    {
        public string Title { get; set; }
        public string FullText { get; set; }
        public DateTime PublicationDate { get; set; }
        public double Rate { get; set; }
        public string SourceName { get; set; }
    }
}
