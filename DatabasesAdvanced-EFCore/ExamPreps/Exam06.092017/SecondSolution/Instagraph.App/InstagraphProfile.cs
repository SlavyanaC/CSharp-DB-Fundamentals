using AutoMapper;

using Instagraph.Models;
using Import = Instagraph.DataProcessor.Dto.Import;
using Export = Instagraph.DataProcessor.Dto.Export;
using System.Linq;

namespace Instagraph.App
{
    public class InstagraphProfile : Profile
    {
        public InstagraphProfile()
        {
            CreateMap<Import.PictureDto, Picture>();

            CreateMap<Import.UserDto, User>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore());

            CreateMap<Post, Export.PostDto>()
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(p => p.Picture.Path))
                .ForMember(dest => dest.User, opt => opt.MapFrom(p => p.User.Username));

            CreateMap<User, Export.PopularUserDto>()
                .ForMember(dest => dest.Followers, opt => opt.MapFrom(u => u.Followers.Count));

            CreateMap<User, Export.UserCommentDto>()
                .ForMember(dest => dest.MostComments, opt => opt.MapFrom(u => u.Posts.Select(p => p.Comments.Count).DefaultIfEmpty(0).Max()));
        }
    }
}
