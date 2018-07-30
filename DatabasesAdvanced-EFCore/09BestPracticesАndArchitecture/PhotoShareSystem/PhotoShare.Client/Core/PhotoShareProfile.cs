namespace PhotoShare.Client.Core
{
    using AutoMapper;
    using Models;
    using Dtos;

    public class PhotoShareProfile : Profile
    {
        public PhotoShareProfile()
        {
            CreateMap<Town, Town>();

            CreateMap<Town, TownDto>().ReverseMap();

            CreateMap<Album, Album>();

            CreateMap<Album, AlbumDto>().ReverseMap();

            CreateMap<Tag, Tag>();

            CreateMap<Tag, TagDto>().ReverseMap();

            CreateMap<AlbumRole, AlbumRoleDto>()
                    .ForMember(dest => dest.AlbumName, from => from.MapFrom(p => p.Album.Name))
                    .ForMember(dest => dest.Username, from => from.MapFrom(p => p.User.Username))
                    .ReverseMap();

            CreateMap<User, User>();

            CreateMap<User, UserDto>();

            CreateMap<User, UserFriendsDto>()
                .ForMember(dto => dto.Friends, opt => opt.MapFrom(u => u.FriendsAdded));

            CreateMap<Friendship, FriendDto>()
                .ForMember(dto => dto.Username, opt => opt.MapFrom(f => f.Friend.Username));
        }
    }
}
