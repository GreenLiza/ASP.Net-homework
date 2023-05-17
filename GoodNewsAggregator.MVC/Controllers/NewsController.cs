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

        public async Task<IActionResult> Index(int page = 1)
        {
            _logger.LogInformation("Index is called");

            var totalNewsCount = await _newsService.GetTotalNewsCountAsync();

            if (int.TryParse(_configuration["Pagination:News:DefaultPageSize"], out int pageSize))
            {
                var pageInfo = new PageInfo()
                {
                    PageSize = pageSize,
                    PageNumber = page,
                    TotalElements = totalNewsCount
                };
                List<NewsPreviewDTO> newsDtos = await _newsService.GetNewsByPageAsync(page, pageSize);
                List<NewsPreviewModel> newsPreview = newsDtos
                    .Select(Dto => _mapper.Map<NewsPreviewModel>(Dto))
                    .ToList();

                return View(new NewsPreviewWithPaginationModel()
                {
                    NewsPreviews = newsPreview,
                    PageInfo = pageInfo
                });

            }
            else
            {
                return Content($"Configuration doesn't exist");
            }

        }

        [Authorize]
        public async Task<IActionResult> Details(string title, int page)
        {
            var newsDto = await _newsService.GetNewsByTitleAsync(title);

            if (newsDto != null)
            {
                NewsModel newsModel = _mapper.Map<NewsModel>(newsDto);
                NewsDetailsViewModel newsDetailsViewModel = new NewsDetailsViewModel()
                {
                    NewsModel = newsModel,
                    PageNumber = page,
                };
                return View(newsDetailsViewModel);
            }
            return Content($"Article {title} doesn't exist");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var model = new NewsCreateModel();
            //{
            //    AvailableSources = (await _sourceService.GetSourcesAsync())
            //        .Select(source => new SelectListItem(source.Name, source.Name))
            //        .ToList()
            //};
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(NewsCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var sourceName = "Good News";
                var sourceId = await _sourceService.GetSourceId(sourceName);
                if (sourceId == 0)
                {
                    await _sourceService.AddDefaultSource();
                    sourceId = await _sourceService.GetSourceId(sourceName);
                }
                var fullNewsDto = new FullNewsDTO()
                {
                    LinkToSource = "",
                    Title = model.Title,
                    ShortDescription = model.ShortDescription,
                    FullText = model.FullText,
                    PublicationDate = DateTime.Now,
                    Rate = 0,
                    SourceName = sourceName,
                    SourceId = sourceId
                    //SourceName = model.SourceName,
                    //SourceId = _sourceService.GetSourceId(model.SourceName)
                };
                await _newsService.CreateNewsAsync(fullNewsDto);
                return RedirectToAction("Index", "News");

            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task GetWithRss()
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
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int page = 1)
        {
            _logger.LogInformation("Update is called");

            var totalNewsCount = await _newsService.GetTotalNewsCountAsync();

            if (int.TryParse(_configuration["Pagination:News:UpdateNewsPageSize"], out int pageSize))
            {
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
            else
            {
                return Content($"Configuration doesn't exist");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string title)
        {
            var newsDto = await _newsService.GetNewsByTitleAsync(title);
            var model = _mapper.Map<NewsEditModel>(newsDto);
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(NewsEditModel model)
        {
            if (ModelState.IsValid)
            {
                var newsDto = _mapper.Map<EditNewsDTO>(model);
                await _newsService.EditNewsArticleAsync(newsDto);
                return RedirectToAction("Update", "News");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(string title)
        {
            await _newsService.RemoveNewsByTitleAsync(title);
            return RedirectToAction("Update", "News");
        }
    }
}
