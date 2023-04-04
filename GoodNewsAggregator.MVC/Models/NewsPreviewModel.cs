namespace GoodNewsAggregator.MVC.Models
{
    public class NewsPreviewModel
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public DateTime PublicationDate { get; set; }
        public double Rate { get; set; }
        public string SourceName { get; set; }
    }
}
