using API_Movies.Models;
using API_Movies.Models.Dtos;
using AutoMapper;

namespace API_Movies.MoviesMapper
{
    public class MoviesMapper : Profile
    {
        public MoviesMapper()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Movie, MovieDto>().ReverseMap();
            CreateMap<Movie, MovieCreateDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AppUser, UserDataDto>().ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();
        }
    }
}
