using AutoMapper;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Contracts;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Domain;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserRequest, User>()
                .ForMember((dest) => dest.Id, (opt) => opt.Ignore())
                .ForMember((dest) => dest.CreatedAt, (opt) => opt.Ignore())
                .ForMember((dest) => dest.UpdatedAt, (opt) => opt.Ignore());
            CreateMap<UpdateUserRequest, User>()
                .ForMember((dest) => dest.Id, (opt) => opt.Ignore())
                .ForMember((dest) => dest.CreatedAt, (opt) => opt.Ignore())
                .ForMember((dest) => dest.UpdatedAt, (opt) => opt.Ignore());
        }
    }
}
