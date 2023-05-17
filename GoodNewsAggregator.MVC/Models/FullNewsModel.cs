using GoodNewsAggregator.Data.Entities;

namespace GoodNewsAggregator.MVC.Models
{
    public class FullNewsModel
    {
        public NewsModel NewsModel { get; set; }
        public string ShortDescription { get; set; }
    }
}
