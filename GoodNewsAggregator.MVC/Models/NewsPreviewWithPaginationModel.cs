namespace GoodNewsAggregator.MVC.Models
{
    public class NewsPreviewWithPaginationModel
    {
        public List<NewsPreviewModel> NewsPreviews { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
