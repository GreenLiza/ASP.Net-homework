using GoodNewsAggregator.DTO;

namespace GoodNewsAggregator.Abstractions.Services
{
    public interface INewsService
    {
        Task<List<NewsPreviewDTO>> GetNewsByPageAsync(int page, int pageSize);
        Task<List<NewsPreviewDTO>> GetNewsListAsync();
        Task<List<FullNewsDTO>> GetFullNewsByPageAsync(int page, int pageSize);
        Task<FullNewsWithIdDTO> GetNewsByTitleAsync(string title);
        Task<FullNewsWithIdDTO> GetNewsByIdAsync(int id);
        Task<int> GetTotalNewsCountAsync();
        Task CreateNewsAsync(FullNewsDTO fullNewsDTO);
        Task EditNewsArticleAsync(EditNewsDTO editNewsDTO);
        Task RemoveNewsByTitleAsync(string title);
        Task<List<FullNewsDTO>> GetNewsFromSourceRss(string link);
        Task<double> RateNewsTextAsync(string? newsText);

    }
}