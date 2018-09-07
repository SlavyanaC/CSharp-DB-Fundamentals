namespace VaporStore.DataProcessor
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using Newtonsoft.Json;
    using Data;
    using Data.Models;
    using DataProcessor.ImportDto;

    public static class Deserializer
    {
        private const string ERROR_MESSAGE = "Invalid Data";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            GameDto[] gameDtos = JsonConvert.DeserializeObject<GameDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();

            List<Game> validGames = new List<Game>();

            List<Developer> developers = new List<Developer>();
            List<Genre> genres = new List<Genre>();
            List<Tag> tags = new List<Tag>();

            foreach (var gameDto in gameDtos)
            {
                if (!IsValid(gameDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                Developer developer = GetDeveloper(developers, gameDto);
                Genre genre = GetGenre(genres, gameDto);

                Game game = Mapper.Map<Game>(gameDto);
                game.Developer = developer;
                game.Genre = genre;

                foreach (var tagName in gameDto.Tags)
                {
                    Tag tag = GetTag(tags, tagName);
                    game.GameTags.Add(new GameTag() { Game = game, Tag = tag });
                }

                validGames.Add(game);
                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

            context.Games.AddRange(validGames);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static Developer GetDeveloper(List<Developer> developers, GameDto gameDto)
        {
            Developer developer = developers.SingleOrDefault(d => d.Name == gameDto.Developer);
            if (developer == null)
            {
                developer = new Developer()
                {
                    Name = gameDto.Developer,
                };

                developers.Add(developer);
            }

            return developer;
        }

        private static Genre GetGenre(List<Genre> genres, GameDto gameDto)
        {
            Genre genre = genres.SingleOrDefault(g => g.Name == gameDto.Genre);
            if (genre == null)
            {
                genre = new Genre()
                {
                    Name = gameDto.Genre,
                };

                genres.Add(genre);
            }

            return genre;
        }

        private static Tag GetTag(List<Tag> tags, string tagName)
        {
            Tag tag = tags.SingleOrDefault(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag()
                {
                    Name = tagName,
                };

                tags.Add(tag);
            }

            return tag;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            UserDto[] userDtos = JsonConvert.DeserializeObject<UserDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            List<User> validUsers = new List<User>();

            foreach (var userDto in userDtos)
            {
                bool cardsAreValid = userDto.Cards.All(IsValid);
                if (!IsValid(userDto) || !cardsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                User user = Mapper.Map<User>(userDto);
                userDto.Cards.ToList().ForEach(c => Mapper.Map<Card>(c));

                validUsers.Add(user);
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PurchaseDto[]), new XmlRootAttribute("Purchases"));
            PurchaseDto[] purchaseDtos = (PurchaseDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();
            List<Purchase> validPurchases = new List<Purchase>();

            foreach (var purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                Purchase purchase = Mapper.Map<Purchase>(purchaseDto);
                purchase.CardId = context.Cards.SingleOrDefault(c => c.Number == purchaseDto.Card).Id;
                purchase.GameId = context.Games.SingleOrDefault(c => c.Name == purchaseDto.Title).Id;
                purchase.Card = context.Cards.SingleOrDefault(c => c.Number == purchaseDto.Card);
                purchase.Game = context.Games.SingleOrDefault(c => c.Name == purchaseDto.Title);

                validPurchases.Add(purchase);
                sb.AppendLine($"Imported {purchaseDto.Title} for {purchase.Card.User.Username}");
            }

            context.Purchases.AddRange(validPurchases);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}