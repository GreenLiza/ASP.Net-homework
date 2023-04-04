using GoodNewsAggregator.DTO;
using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Data.Entities;
using Azure;

namespace GoodNewsAggregator.Business
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public NewsService(IUnitOfWork unitOfWork, IMapper mapper)
        { 
            _unitOfWork= unitOfWork;
            _mapper = mapper;
        }

        public async Task<NewsDTO?> GetNewsByIdAsync(int id)
        {
            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news != null)
            {
                var newsDto = _mapper.Map<NewsDTO>(news);
                return newsDto;
            }
            return null;
        }

        public async Task<NewsDTO?> GetNewsByTitleAsync(string title)
        {
            var news = await _unitOfWork.News.GetNewsByTitleAsync(title);
            if (news != null)
            {
                var newsDto = _mapper.Map<NewsDTO>(news);
                return newsDto;
            }
            return null;
        }

        

        public async Task<List<NewsPreviewDTO>> GetNewsByPageAsync(int page, int pageSize)
        {
            List<NewsPreviewDTO> newsDtos = new List<NewsPreviewDTO>();
            newsDtos = (await _unitOfWork.News.GetNewsByPageAsync(page, pageSize))
                .Select(news => _mapper.Map<NewsPreviewDTO>(news))
                .ToList();
            return newsDtos;
        }

        public async Task<List<NewsPreviewDTO>> GetNewsListAsync()
        {
            List<NewsPreviewDTO> newsDtos = new List<NewsPreviewDTO>();
            newsDtos = (await _unitOfWork.News.GetNewsListAsync())
                .Select(news => _mapper.Map<NewsPreviewDTO>(news))
                .ToList();
            return newsDtos;
        }

        public async Task<int> GetTotalNewsCountAsync()
        {
            var count = await _unitOfWork.News.CountAsync();
            return count;
        }

    }
}