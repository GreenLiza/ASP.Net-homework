namespace GoodNewsAggregator.MVC.Models
{
    public class NewsPreviewWithPageModel : NewsPreviewModel
    {
        public int Page { get; set; }

        public NewsPreviewWithPageModel(NewsPreviewModel newsPreviewModel, int page)
        {
            Title = newsPreviewModel.Title;
            ShortDescription = newsPreviewModel.ShortDescription;
            PublicationDate = newsPreviewModel.PublicationDate;
            Rate = newsPreviewModel.Rate;
            SourceName = newsPreviewModel.SourceName;
            Page = page;
        }
    }
}
