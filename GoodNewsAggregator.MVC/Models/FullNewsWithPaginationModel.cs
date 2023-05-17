namespace GoodNewsAggregator.MVC.Models
{
    public class FullNewsWithPaginationModel
    {
        public List<FullNewsModel> FullNews { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
