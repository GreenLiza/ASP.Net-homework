using AutoMapper;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using GoodNewsAggregator.MVC.Models;

namespace GoodNewsAggregator.MVC.Mapping
{
    public class SourceProfile : Profile
    {
        public SourceProfile()
        {
            CreateMap<SourceDto, Source>().ReverseMap();
            
            

        }
    }
}
