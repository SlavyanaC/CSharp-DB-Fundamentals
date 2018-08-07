namespace Instagraph.App
{
    using AutoMapper;
    using Instagraph.DataProcessor.Dtos.Import;
    using Instagraph.DataProcessor.Dtos.Export;
    using Instagraph.Models;

    public class InstagraphProfile : Profile
    {
        public InstagraphProfile()
        {
            CreateMap<PictureDto, Picture>();

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore());

            CreateMap<UserFollowerDto, UserFollower>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Follower, opt => opt.Ignore());

            CreateMap<PostDto, Post>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Picture, opt => opt.Ignore());

            CreateMap<CommentDto, Comment>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore());

            CreateMap<User, PopularUserDto>()
                .ForMember(dest => dest.Followers, opt => opt.MapFrom(u => u.Followers.Count));
        }
    }
}
