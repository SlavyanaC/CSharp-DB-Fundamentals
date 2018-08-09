namespace Stations.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    using AutoMapper;
    using Newtonsoft.Json;

    using Stations.Data;
    using Stations.DataProcessor.Dto.Import;
    using Stations.DataProcessor.Dto.Import.Ticket;
    using Stations.Models;
    using Stations.Models.Enums;

    public static class Deserializer
    {
        private const string ERROR_MESSAGE = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportStations(StationsDbContext context, string jsonString)
        {
            var stationDtos = JsonConvert.DeserializeObject<StationDto[]>(jsonString);

            var sb = new StringBuilder();
            var validStations = new List<Station>();

            foreach (var stationDto in stationDtos)
            {
                bool exists = validStations.Any(s => s.Name == stationDto.Name);
                if (!IsValid(stationDto) || exists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                stationDto.Town = stationDto.Town ?? stationDto.Name;

                var station = Mapper.Map<Station>(stationDto);

                validStations.Add(station);
                sb.AppendLine(string.Format(SuccessMessage, station.Name));
            }

            context.Stations.AddRange(validStations);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportClasses(StationsDbContext context, string jsonString)
        {
            var classDtos = JsonConvert.DeserializeObject<SeatingClassDto[]>(jsonString);

            var sb = new StringBuilder();
            var validClasses = new List<SeatingClass>();

            foreach (var classDto in classDtos)
            {
                bool exists = validClasses.Any(c => c.Name == classDto.Name || c.Abbreviation == classDto.Abbreviation);
                if (!IsValid(classDto) || exists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var seatingClass = Mapper.Map<SeatingClass>(classDto);

                validClasses.Add(seatingClass);
                sb.AppendLine(string.Format(SuccessMessage, seatingClass.Name));
            }

            context.SeatingClasses.AddRange(validClasses);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportTrains(StationsDbContext context, string jsonString)
        {
            var trainDtos = JsonConvert.DeserializeObject<TrainDto[]>(jsonString, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });

            var sb = new StringBuilder();
            var validTrains = new List<Train>();

            foreach (var trainDto in trainDtos)
            {
                var seatsAreValid = trainDto.Seats.All(IsValid);
                bool exists = validTrains.Any(t => t.TrainNumber == trainDto.TrainNumber);
                var classesAreValid = trainDto.Seats.All(s => context.SeatingClasses.Any(sc => sc.Name == s.Name && sc.Abbreviation == s.Abbreviation));

                if (!IsValid(trainDto) || !seatsAreValid || !classesAreValid || exists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var trainType = Enum.Parse<TrainType>(trainDto.Type);

                var trainSeats = trainDto.Seats.Select(s => new TrainSeat
                {
                    SeatingClass = context.SeatingClasses.SingleOrDefault(sc => sc.Name == s.Name && sc.Abbreviation == s.Abbreviation),
                    Quantity = s.Quantity.Value,
                })
                .ToArray();

                var train = new Train
                {
                    TrainNumber = trainDto.TrainNumber,
                    Type = trainType,
                    TrainSeats = trainSeats,
                };

                validTrains.Add(train);
                sb.AppendLine(string.Format(SuccessMessage, train.TrainNumber));
            }

            context.Trains.AddRange(validTrains);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportTrips(StationsDbContext context, string jsonString)
        {
            var tripDtos = JsonConvert.DeserializeObject<TripDto[]>(jsonString);

            var sb = new StringBuilder();
            var validTrips = new List<Trip>();

            foreach (var tripDto in tripDtos)
            {
                if (!IsValid(tripDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var train = context.Trains.SingleOrDefault(t => t.TrainNumber == tripDto.TrainNumber);
                var originStation = context.Stations.SingleOrDefault(s => s.Name == tripDto.OriginStationName);
                var destinationStation = context.Stations.SingleOrDefault(s => s.Name == tripDto.DestinationStationName);

                if (train == null || originStation == null || destinationStation == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var departureTime = DateTime.ParseExact(tripDto.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var arrivalTime = DateTime.ParseExact(tripDto.ArrivalTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                if (departureTime > arrivalTime)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                TimeSpan timeDiff;
                if (tripDto.TimeDifference != null)
                {
                    timeDiff = TimeSpan.ParseExact(tripDto.TimeDifference, "hh\\:mm", CultureInfo.InvariantCulture);
                }

                var tripStatus = Enum.Parse<TripStatus>(tripDto.Status);

                var trip = new Trip
                {
                    Train = train,
                    OriginStation = originStation,
                    DestinationStation = destinationStation,
                    DepartureTime = departureTime,
                    ArrivalTime = arrivalTime,
                    TimeDifference = timeDiff,
                    Status = tripStatus,
                };

                validTrips.Add(trip);
                sb.AppendLine($"Trip from {tripDto.OriginStationName} to {tripDto.DestinationStationName} imported.");
            }

            context.Trips.AddRange(validTrips);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportCards(StationsDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(CardDto[]), new XmlRootAttribute("Cards"));
            var carDtos = (CardDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validCards = new List<CustomerCard>();

            foreach (var cardDto in carDtos)
            {
                if (!IsValid(cardDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var cardType = Enum.Parse<CardType>(cardDto.CardType);

                var card = Mapper.Map<CustomerCard>(cardDto);
                card.Type = cardType;

                validCards.Add(card);
                sb.AppendLine(string.Format(SuccessMessage, card.Name));
            }

            context.Cards.AddRange(validCards);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportTickets(StationsDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(TicketDto[]), new XmlRootAttribute("Tickets"));
            var ticketDtos = (TicketDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validTickets = new List<Ticket>();

            foreach (var ticketDto in ticketDtos)
            {
                if (!IsValid(ticketDto) || !IsValid(ticketDto.Trip))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var ticketDepartureTime = DateTime.ParseExact(ticketDto.Trip.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                var trip = context.Trips
                    .SingleOrDefault(t => t.OriginStation.Name == ticketDto.Trip.OriginStationName &&
                                     t.DestinationStation.Name == ticketDto.Trip.DestinationStationName &&
                                     t.DepartureTime == ticketDepartureTime);

                if (trip == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var train = context.Trains.SingleOrDefault(t => t.TrainNumber == trip.Train.TrainNumber);

                var seatingClassName = ticketDto.SeatingPlace.Substring(0, 2);
                var seatNumber = int.Parse(ticketDto.SeatingPlace.Substring(2));

                var trainContainsSeatingClass = train.TrainSeats.Any(ts => ts.SeatingClass.Abbreviation == seatingClassName);
                var trainNumberOfPlaces = train.TrainSeats.Where(t => t.SeatingClass.Abbreviation == seatingClassName).Select(t => t.Quantity).SingleOrDefault();

                var trainHasPlace = seatNumber > 0 && seatNumber <= trainNumberOfPlaces;

                if (!trainContainsSeatingClass || !trainHasPlace)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                CustomerCard cardFromDb = null;
                if (ticketDto.Card != null)
                {
                    var card = context.Cards.SingleOrDefault(c => c.Name == ticketDto.Card.Name);
                    if (card == null || !IsValid(ticketDto))
                    {
                        sb.AppendLine(ERROR_MESSAGE);
                        continue;
                    }

                    cardFromDb = card;
                }

                var ticket = Mapper.Map<Ticket>(ticketDto);
                ticket.CustomerCard = cardFromDb;
                ticket.Trip = trip;

                validTickets.Add(ticket);

                var originStatin = ticketDto.Trip.OriginStationName;
                var destinationStation = ticketDto.Trip.DestinationStationName;
                var departureTime = ticketDepartureTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                sb.AppendLine($"Ticket from {originStatin} to {destinationStation} departing at {departureTime} imported.");
            }

            context.Tickets.AddRange(validTickets);
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