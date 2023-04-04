using GoodNewsAggregator.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using GoodNewsAggregator.DTO;
using AutoMapper;
using GoodNewsAggregator.Abstractions.Services;

namespace GoodNewsAggregator.MVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public NewsController(INewsService newsService, IMapper mapper, IConfiguration configuration)
        {
            _newsService = newsService;
            _mapper = mapper;
            _configuration = configuration;
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


            //await _newsService.AddAsync(new NewsDTO());

        }

        public async Task<IActionResult> Details([FromRoute] NewsSearchModel val)
        {
            var newsDto = await _newsService.GetNewsByTitleAsync(val.Title);

            if (newsDto != null)
            {
                NewsModel newsModel = _mapper.Map<NewsModel>(newsDto);
                return View(newsModel);
            }
            return Content($"Article {val.Title} doesn't exist");
        }

    }
}
