using AutoMapper;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using GoodNewsAggregator.MVC.Models;

namespace GoodNewsAggregator.MVC.Mapping
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<NewsPreviewDTO, NewsPreviewModel>();
            CreateMap<News, NewsPreviewDTO>()
                .ForMember(dst => dst.SourceName, opt => opt.MapFrom(src => src.Source.Name));
            CreateMap<News, NewsDTO>()
                .ForMember(dst => dst.SourceName, opt => opt.MapFrom(src => src.Source.Name));
            CreateMap<NewsDTO, NewsModel>();
            CreateMap<FullNewsDTO, News>();
                        
        }
    }
}
