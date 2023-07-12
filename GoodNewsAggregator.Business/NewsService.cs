using GoodNewsAggregator.DTO;
using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using GoodNewsAggregator.Abstractions;
using GoodNewsAggregator.Data.Entities;
using Azure;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Net.Http.Headers;
using HtmlAgilityPack;
using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using GoodNewsAggregator.Business.RateModels;
using System.Collections.Generic;
using System.Collections;
using GoodNewsAggregator.Data.Migrations;

namespace GoodNewsAggregator.Business
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISourceService _sourceService;


        public NewsService(IUnitOfWork unitOfWork, IMapper mapper, ISourceService sourceService)
        {
            _unitOfWork = unitOfWork;
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
                    var rate = RateNewsTextAsync(text);
                    if (rate.Result >= 0)
                    {
                        newsDtos.Add(new FullNewsDTO()
                        {
                            LinkToSource = item.Id,
                            Title = item.Title.Text,
                            ShortDescription = item.Summary.Text,
                            FullText = text,
                            PublicationDate = item.PublishDate.DateTime,
                            Rate = rate.Result,
                            SourceName = source.Name,
                            SourceId = source.Id
                        });
                    }
                }
            }
            return newsDtos;
        }

        public async Task<double> RateNewsTextAsync(string newsText)
        {
            var ratingWords = GetRatingWordsDictionary();
            newsText = CleanText(newsText);

            var lemmas = await GetTextLemmasAsync(newsText);

            if (lemmas.Any())
            {
                var totalRate = lemmas.Where(lemma => ratingWords.ContainsKey(lemma))
                    .Aggregate<string, double>(0, (current, lemma)
                        => current + ratingWords[lemma]);

                var rate = totalRate / lemmas.Count();
                switch (rate)
                {
                    case < 0:
                        rate = -1;
                        break;
                    case < 0.01:
                        rate = 0;
                        break;
                    case < 0.03:
                        rate = 1;
                        break;
                    case < 0.05:
                        rate = 2;
                        break;
                    case < 0.07:
                        rate = 3;
                        break;
                    case < 0.09:
                        rate = 4;
                        break;
                    default:
                        rate = 5;
                        break;
                }
                return rate;
            }
            else
            {
                return 0;
            }
        }

        private string GetNewsArticleFullTextAsync(string link)
        {
            var web = new HtmlWeb();
            var doc = web.Load(link);
            HtmlNode? text;
            text = doc.DocumentNode.SelectSingleNode("//div[@class = 'news-text']");
            if (text != null)
            {
                var nodesToRemove = doc.DocumentNode.SelectNodes("//div[@class = 'news-reference']").ToList();
                foreach (var node in nodesToRemove)
                {
                    node.Remove();
                }
                var checkNode = doc.DocumentNode.SelectSingleNode("//div[@class = 'news-widget']");
                if (checkNode is not null)
                {
                    nodesToRemove = doc.DocumentNode.SelectNodes("//div[@class = 'news-widget']").ToList();
                    foreach (var node in nodesToRemove)
                    {
                        node.Remove();
                    }
                }
            }
            else
            {
                text = doc.DocumentNode.SelectSingleNode("//div[@class = 'text']");
            }
            var fullText = text.InnerHtml;
            return fullText;
        }

        private string CleanText(string newsText)
        {
            newsText = newsText.Trim();
            newsText = Regex.Replace(newsText, "<.*?>", string.Empty);
            Regex r = new Regex("(?:[^а-яё0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(newsText, String.Empty);
            //newsText = newsText.Trim(new Char[] {' ', '*', '.', '?', '&', '{', '}', '[', ']'}).Replace('\n', ' ').Replace('\"', ' ').Replace('\'', ' ');
        }

        private Dictionary<string, int>? GetRatingWordsDictionary()
        {
            Dictionary<string, int>? dictionary;
            using (var jsonReader = new StreamReader(@"C:\Users\user\source\repos\GoodNewsAggregator\AFINN-ru.json"))
            {
                var json = jsonReader.ReadToEnd();
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
            return dictionary;
        }

        private async Task<string[]>? GetTextLemmasAsync(string newsText)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var request = new HttpRequestMessage(HttpMethod.Post,
                    "http://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey=0c51366ce118cb3e7bd113dc04fbaf717d0bec66")
                {
                    Content = new StringContent("[{\"text\":\"" + newsText + "\"}]",
                        Encoding.UTF8, "application/json")
                };

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responceString = await response.Content.ReadAsStringAsync();
                    var lemmas = JsonConvert.DeserializeObject<Root[]>(responceString)
                        .SelectMany(root => root.Annotations.Lemma).Select(lemma => lemma.Value).ToArray();
                    return lemmas;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}