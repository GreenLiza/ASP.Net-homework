using GoodNewsAggregator.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using GoodNewsAggregator.DTO;
using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper.Configuration.Annotations;
using System.Reflection.PortableExecutable;
using System.ServiceModel.Syndication;
using System.Xml;
using System;
using GoodNewsAggregator.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.MVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly ISourceService _sourceService;
        private readonly ILogger<NewsController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public NewsController(INewsService newsService, IMapper mapper, IConfiguration configuration, ISourceService sourceService, ILogger<NewsController> logger)
        {
            _newsService = newsService;
            _mapper = mapper;
            _configuration = configuration;
            _sourceService = sourceService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, string sortBy = "default")
        {
            _logger.LogInformation("Index page is called");

            try
            {
                var totalNewsCount = await _newsService.GetTotalNewsCountAsync();
                int.TryParse(_configuration["Pagination:News:DefaultPageSize"], out int pageSize);
                if (pageSize == 0)
                {
                    pageSize = 6;
                }

                var pageInfo = new PageInfo()
                {
                    PageSize = pageSize,
                    PageNumber = page,
                    TotalElements = totalNewsCount
                };

                //List<NewsPreviewDTO> newsDtos = await _newsService.GetNewsByPageAsync(page, pageSize);
                List<NewsPreviewDTO> newsDtos = await _newsService.GetNewsListAsync();
                List<NewsPreviewModel> newsPreview = newsDtos
                    .Select(Dto => _mapper.Map<NewsPreviewModel>(Dto))
                    .ToList();

                switch (sortBy)
                {
                    case "date":
                        newsPreview = newsPreview.OrderByDescending(o => o.PublicationDate).ToList();
                        break;
                    case "rate":
                        newsPreview = newsPreview.OrderByDescending(o => o.Rate).ToList();
                        break;
                    case "default":
                        break;
                }

                newsPreview = newsPreview.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return View(new NewsPreviewWithAdditionalInfoModel()
                {
                    SortBy = sortBy,
                    NewsPreviews = newsPreview,
                    PageInfo = pageInfo
                });

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");   
            }

        }

        [Authorize]
        public async Task<IActionResult> Details(string title, int page)
        {
            _logger.LogInformation("Details page is called");

            try
            {
                var newsDto = await _newsService.GetNewsByTitleAsync(title);
                NewsModel newsModel = _mapper.Map<NewsModel>(newsDto);
                NewsDetailsViewModel newsDetailsViewModel = new NewsDetailsViewModel()
                {
                    NewsModel = newsModel,
                    PageNumber = page,
                };
                return View(newsDetailsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create page is called");

            var model = new NewsCreateModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(NewsCreateModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("New news article content passed validation");
                    var sourceName = "Good News";
                    var sourceId = await _sourceService.GetSourceId(sourceName);
                    if (sourceId == 0)
                    {
                        await _sourceService.AddDefaultSource();
                        sourceId = await _sourceService.GetSourceId(sourceName);
                    }
                    var rate = _newsService.RateNewsTextAsync(model.FullText);
                    var fullNewsDto = new FullNewsDTO()
                    {
                        LinkToSource = "",
                        Title = model.Title,
                        ShortDescription = model.ShortDescription,
                        FullText = model.FullText,
                        PublicationDate = DateTime.Now,
                        Rate = rate.Result,
                        SourceName = sourceName,
                        SourceId = sourceId
                    };
                    await _newsService.CreateNewsAsync(fullNewsDto);
                    _logger.LogInformation("New news article is created");
                    return RedirectToAction("Index", "News");

                }
                _logger.LogInformation("New news article content failed validation");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWithRss()
        {
            _logger.LogInformation("News collection has started");

            try
            {
                var rssLinks = await _sourceService.GetSourcesRssLinksAsync();
                foreach (string link in rssLinks)
                {
                    var fullNewsDtos = await _newsService.GetNewsFromSourceRss(link);
                    foreach (var fullNewsDto in fullNewsDtos)
                    {
                        if ((await _newsService.GetNewsByTitleAsync(fullNewsDto.Title)) == null)
                        {
                            await _newsService.CreateNewsAsync(fullNewsDto);
                        }
                    }
                }
                _logger.LogInformation("News collection is successful");
                return RedirectToAction("Update", "News");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int page = 1)
        {
            _logger.LogInformation("Update page is called");

            try
            {
                var totalNewsCount = await _newsService.GetTotalNewsCountAsync();

                int.TryParse(_configuration["Pagination:News:UpdateNewsPageSize"], out int pageSize);
                if (pageSize == 0)
                {
                    pageSize = 3;
                }
                var pageInfo = new PageInfo()
                {
                    PageSize = pageSize,
                    PageNumber = page,
                    TotalElements = totalNewsCount
                };

                List<FullNewsDTO> newsDtos = await _newsService.GetFullNewsByPageAsync(page, pageSize);

                List<FullNewsModel> fullNews = newsDtos
                   .Select(Dto => new FullNewsModel()
                   {
                       NewsModel = _mapper.Map<NewsModel>(Dto),
                       ShortDescription = Dto.ShortDescription,
                   })
                   .ToList();

                return View(new FullNewsWithPaginationModel()
                {
                    FullNews = fullNews,
                    PageInfo = pageInfo
                });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string title)
        {
            _logger.LogInformation("Edit page is called");
            try
            {
                var newsDto = await _newsService.GetNewsByTitleAsync(title);
                var model = _mapper.Map<NewsEditModel>(newsDto);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(NewsEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Edited news article content passed validation");
                    var newsDto = _mapper.Map<EditNewsDTO>(model);
                    await _newsService.EditNewsArticleAsync(newsDto);
                    return RedirectToAction("Update", "News");
                }
                _logger.LogInformation("Edited news article content failed validation");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(string title)
        {
            try
            {
                await _newsService.RemoveNewsByTitleAsync(title);
                _logger.LogInformation("News article was removed");
                return RedirectToAction("Update", "News");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
