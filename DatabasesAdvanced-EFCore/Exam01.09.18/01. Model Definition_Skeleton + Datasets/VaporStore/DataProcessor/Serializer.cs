namespace VaporStore.DataProcessor
{
    using System;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Newtonsoft.Json;
    using Data;
    using ExportDto;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Text;
    using System.IO;
    using VaporStore.Data.Models.Enums;
    using System.Globalization;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            //Doesn't work in Judge
            GenreDto[] genreDtos = context.Genres
                .Where(g => genreNames.Contains(g.Name))
                .ProjectTo<GenreDto>()
                .OrderByDescending(g => g.Games.Sum(ga => ga.Players))
                .ThenBy(g => g.Id)
                .ToArray();

            //GenreDto[] genreDtos = context.Genres
            //    .Where(g => genreNames.Contains(g.Name))
            //    .Select(g => new GenreDto
            //    {
            //        Id = g.Id,
            //        Genre = g.Name,
            //        Games = g.Games
            //            .Where(ga => ga.Purchases.Any())
            //            .Select(ga => new GameDto
            //            {
            //                Id = ga.Id,
            //                Title = ga.Name,
            //                Developer = ga.Developer.Name,
            //                Tags = string.Join(", ", ga.GameTags.Select(t => t.Tag.Name)),
            //                Players = ga.Purchases.Count
            //            })
            //            .OrderByDescending(ga => ga.Players)
            //            .ThenBy(ga => ga.Id)
            //            .ToArray(),
            //        TotalPlayers = g.Games.Sum(ga => ga.Purchases.Count)
            //    })
            //    .OrderByDescending(g => g.TotalPlayers)
            //    .ThenBy(g => g.Id)
            //    .ToArray();

            var jsonString = JsonConvert.SerializeObject(genreDtos, Newtonsoft.Json.Formatting.Indented);

            return jsonString;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            PurchaseType purchaseType = Enum.Parse<PurchaseType>(storeType);

            var purchases = context.Users
                 .Select(u => new UserDto
                 {
                     Username = u.Username,
                     Purchases = u.Cards
                         .SelectMany(c => c.Purchases)
                         .Where(p => p.Type == purchaseType)
                         .Select(p => new PurchaseDto
                         {
                             Card = p.Card.Number,
                             Cvc = p.Card.Cvc,
                             Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                             Game = new UserGameDto
                             {
                                 Title = p.Game.Name,
                                 Genre = p.Game.Genre.Name,
                                 Price = p.Game.Price,
                             }
                         })
                         .OrderBy(p => p.Date)
                         .ToArray(),
                     TotalSpent = u.Cards
                         .SelectMany(c => c.Purchases)
                         .Where(p => p.Type == purchaseType)
                         .Sum(p => p.Game.Price)
                 })
                 .Where(u => u.Purchases.Any())
                 .OrderByDescending(u => u.TotalSpent)
                 .ThenBy(u => u.Username)
                 .ToArray();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(UserDto[]), new XmlRootAttribute("Users"));

            StringBuilder sb = new StringBuilder();
            serializer.Serialize(new StringWriter(sb), purchases, namespaces);

            return sb.ToString();
        }
    }
}