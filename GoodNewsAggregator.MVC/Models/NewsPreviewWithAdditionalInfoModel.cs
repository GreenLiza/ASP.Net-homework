namespace GoodNewsAggregator.MVC.Models
{
    public class NewsPreviewWithAdditionalInfoModel
    {
        public string SortBy { get; set; }
        public List<NewsPreviewModel> NewsPreviews { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
