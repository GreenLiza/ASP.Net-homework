using GoodNewsAggregator.DTO;
using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Data.Entities;
using Azure;
using System.Xml;
using System.ServiceModel.Syndication;
using HtmlAgilityPack;
using System;

namespace GoodNewsAggregator.Business
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISourceService _sourceService;


        public NewsService(IUnitOfWork unitOfWork, IMapper mapper, ISourceService sourceService)
        { 
            _unitOfWork= unitOfWork;
            _mapper = mapper;
            _sourceService = sourceService;
        }

        public async Task<FullNewsWithIdDTO?> GetNewsByIdAsync(int id)
        {
            var news = await _unitOfWork.News.GetByIdAsync(id);
            if (news != null)
            {
                var newsDto = _mapper.Map<FullNewsWithIdDTO>(news);
                return newsDto;
            }
            return null;
        }

        public async Task<FullNewsWithIdDTO?> GetNewsByTitleAsync(string title)
        {
            var news = await _unitOfWork.News.GetNewsByTitleAsync(title);
            if (news != null)
            {
                var newsDto = _mapper.Map<FullNewsWithIdDTO>(news);
                return newsDto;
            }
            return null;
        }

        public async Task<List<FullNewsDTO>> GetFullNewsByPageAsync(int page, int pageSize)
        {
            List<FullNewsDTO> newsDtos = new List<FullNewsDTO>();
            newsDtos = (await _unitOfWork.News.GetNewsByPageAsync(page, pageSize))
                .Select(news => _mapper.Map<FullNewsDTO>(news))
                .ToList();
            return newsDtos;
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

        public async Task CreateNewsAsync(FullNewsDTO fullNewsDTO)
        {
            var news = await _unitOfWork.News.AddAsync(_mapper.Map<News>(fullNewsDTO));
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task EditNewsArticleAsync(EditNewsDTO newsDTO)
        {
            await _unitOfWork.News.EditNewsArticleAsync(_mapper.Map<News>(newsDTO));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveNewsByTitleAsync(string title)
        {
            var news = await _unitOfWork.News.GetNewsByTitleAsync(title);
            await _unitOfWork.News.Remove(news.Id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<FullNewsDTO>> GetNewsFromSourceRss(string link)
        {
            var newsDtos = new List<FullNewsDTO>();
            using (XmlReader reader = XmlReader.Create(link))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (SyndicationItem item in feed.Items)
                {
                    var text = GetNewsArticleFullTextAsync(item.Id);
                    var source = await _sourceService.GetSourceByRssLinkAsync(link);
                    newsDtos.Add(new FullNewsDTO()
                    {
                        LinkToSource = item.Id,
                        Title = item.Title.Text,
                        ShortDescription = item.Summary.Text,
                        FullText = text,
                        PublicationDate = item.PublishDate.DateTime,
                        Rate = 0,
                        SourceName = source.Name,
                        SourceId = source.Id
                    });
                }
            }
            return newsDtos;
        }

        private string GetNewsArticleFullTextAsync(string link)
        {
            var web = new HtmlWeb();
            var doc = web.Load(link);
            HtmlNode? text;
            text = doc.DocumentNode.SelectSingleNode("//div[@class = 'news-text']");
            if (text == null)
            {
                text = doc.DocumentNode.SelectSingleNode("//div[@class = 'text']");
            }
            var fullText = text.InnerHtml;
            return fullText;
        }
    }
}