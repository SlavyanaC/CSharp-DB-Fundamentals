namespace VaporStore
{
    using System;
    using System.Linq;
    using System.Globalization;
    using AutoMapper;
    using Data.Models;
    using Import = DataProcessor.ImportDto;
    using Export = DataProcessor.ExportDto;

    public class VaporStoreProfile : Profile
    {
        public VaporStoreProfile()
        {
            CreateMap<Import.GameDto, Game>()
                .ForMember(d => d.Developer, o => o.Ignore())
                .ForMember(d => d.Genre, o => o.Ignore());

            CreateMap<Import.UserDto, User>();

            CreateMap<Import.CardDto, Card>()
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.User, o => o.Ignore());

            CreateMap<Import.PurchaseDto, Purchase>()
                .ForMember(d => d.ProductKey, o => o.MapFrom(x => x.Key))
                .ForMember(d => d.Date, o => o.MapFrom(x => DateTime.ParseExact(x.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)))
                .ForMember(d => d.CardId, o => o.Ignore())
                .ForMember(d => d.GameId, o => o.Ignore())
                .ForMember(d => d.Game, o => o.Ignore())
                .ForMember(d => d.Card, o => o.Ignore());

            CreateMap<Game, Export.GameDto>()
                .ForMember(d => d.Title, o => o.MapFrom(x => x.Name))
                .ForMember(d => d.Developer, o => o.MapFrom(x => x.Developer.Name))
                .ForMember(d => d.Tags, o => o.MapFrom(x => string.Join(", ", x.GameTags
                    .Select(gt => gt.Tag.Name))))
                .ForMember(d => d.Players, o => o.MapFrom(x => x.Purchases.Count));

            CreateMap<Genre, Export.GenreDto>()
                .ForMember(d => d.Genre, o => o.MapFrom(x => x.Name))
                .ForMember(d => d.Games, o => o.MapFrom(x => x.Games
                    .Where(y => y.Purchases.Any())
                    .OrderByDescending(y => y.Purchases.Count)
                    .ThenBy(y => y.Id)))
                .ForMember(d => d.TotalPlayers, o => o.MapFrom(x => x.Games
                    .Sum(g => g.Purchases.Count)));
        }
    }
}