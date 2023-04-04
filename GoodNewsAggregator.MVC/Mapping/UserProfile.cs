using AutoMapper;
using GoodNewsAggregator.Data.Entities;
using GoodNewsAggregator.DTO;
using GoodNewsAggregator.MVC.Models;

namespace GoodNewsAggregator.MVC.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            

        }
    }
}
