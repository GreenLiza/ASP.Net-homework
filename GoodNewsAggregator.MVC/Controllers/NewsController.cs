using GoodNewsAggregator.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using GoodNewsAggregator.DTO;
using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using GoodNewsAggregator.Data.Entities;

namespace GoodNewsAggregator.MVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public NewsController(INewsService newsService, IMapper mapper, IConfiguration configuration, ISourceService sourceService)
        {
            _newsService = newsService;
            _mapper = mapper;
            _configuration = configuration;
            _sourceService = sourceService;
        }

        public async Task<IActionResult> Index(int page=1)
        {
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
            var model = new NewsCreateModel()
            {
                AvailableSources = (await _sourceService.GetSourcesAsync())
                    .Select(source => new SelectListItem(source.Name, source.Name))
                    .ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(NewsCreateModel model)
        {
            if(ModelState.IsValid)
            {
                var fullNewsDto = new FullNewsDTO()
                {
                    Title = model.Title,
                    ShortDescription = model.ShortDescription,
                    FullText = model.FullText,
                    PublicationDate = DateTime.Now,
                    Rate = 0,
                    SourceName = model.SourceName,
                    SourceId = _sourceService.GetSourceId(model.SourceName)
                };
                await _newsService.CreateNewsAsync(fullNewsDto);
                return RedirectToAction("Index", "News");

            }
            return View();
        }
    }
}
