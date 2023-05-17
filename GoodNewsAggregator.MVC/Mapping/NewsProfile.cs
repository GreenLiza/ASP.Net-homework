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
             CreateMap<News, FullNewsDTO>()
                .ForMember(dst => dst.SourceName, opt => opt.MapFrom(src => src.Source.Name));
             CreateMap<News, FullNewsWithIdDTO>()
                .ForMember(dst => dst.SourceName, opt => opt.MapFrom(src => src.Source.Name));
             CreateMap<FullNewsDTO, NewsModel>();
             CreateMap<FullNewsDTO, News>();
             CreateMap<FullNewsWithIdDTO, NewsEditModel>();
             CreateMap<EditNewsDTO, NewsEditModel>().ReverseMap();
             CreateMap<EditNewsDTO, News>().ReverseMap();
             CreateMap<FullNewsWithIdDTO, NewsModel>().ReverseMap();

        }
    }
}
