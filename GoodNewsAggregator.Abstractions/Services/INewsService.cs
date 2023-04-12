using GoodNewsAggregator.DTO;

namespace GoodNewsAggregator.Abstractions.Services
{
    public interface INewsService
    {
        Task<List<NewsPreviewDTO>> GetNewsByPageAsync(int page, int pageSize);
        Task<List<NewsPreviewDTO>> GetNewsListAsync();
        Task<NewsDTO> GetNewsByTitleAsync(string title);
        Task<NewsDTO> GetNewsByIdAsync(int id);
        Task<int> GetTotalNewsCountAsync();
        Task CreateNewsAsync(FullNewsDTO fullNewsDTO);
        
    }
}