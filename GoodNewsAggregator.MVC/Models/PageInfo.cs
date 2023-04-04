namespace GoodNewsAggregator.MVC.Models
{
    public class PageInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalElements/PageSize);

    }
}
