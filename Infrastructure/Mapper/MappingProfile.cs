using Authentication.Models;
using Authentication.Models.Dtos;
using AutoMapper;

namespace Authentication.Infrastructure.Mapper;

// Created Mapper profile. You dont need to use mapper. It's optional.
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto,User>();
    }
}