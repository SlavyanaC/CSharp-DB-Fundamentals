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
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    using Stations.Data;
    using Stations.DataProcessor.Dto.Import;
    using Stations.Models;
    using Stations.Models.Enums;

    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";
        private const string TripAddMessage = "Trip from {0} to {1} imported.";
        private const string TicketaAddMessage = "Ticket from {0} to {1} departing at {2} imported.";

        public static string ImportStations(StationsDbContext context, string jsonString)
        {
            Station[] deserializedStations = JsonConvert.DeserializeObject<Station[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            List<Station> validStations = new List<Station>();
            foreach (var deserializedStation in deserializedStations)
            {
                if (deserializedStation.Town == null)
                {
                    deserializedStation.Town = deserializedStation.Name;
                }

                var alreadyExists = validStations.Any(s => s.Name == deserializedStation.Name);
                if (!IsValid(deserializedStation) || alreadyExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                if (deserializedStation.Town == null)
                {
                    deserializedStation.Town = deserializedStation.Name;
                }

                Station station = new Station
                {
                    Name = deserializedStation.Name,
                    Town = deserializedStation.Town,
                };

                validStations.Add(station);
                sb.AppendLine(string.Format(SuccessMessage, deserializedStation.Name));
            }

            context.Stations.AddRange(validStations);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportClasses(StationsDbContext context, string jsonString)
        {
            SeatingClass[] deserializedClasses = JsonConvert.DeserializeObject<SeatingClass[]>(jsonString);

            List<SeatingClass> validSeatingClasses = new List<SeatingClass>();
            StringBuilder sb = new StringBuilder();

            foreach (var deserializedClass in deserializedClasses)
            {
                bool isAdded = validSeatingClasses.Any(sc => sc.Name == deserializedClass.Name || sc.Abbreviation == deserializedClass.Abbreviation);
                if (!IsValid(deserializedClass) || isAdded)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                SeatingClass seatingClass = new SeatingClass
                {
                    Name = deserializedClass.Name,
                    Abbreviation = deserializedClass.Abbreviation
                };

                validSeatingClasses.Add(seatingClass);
                sb.AppendLine(string.Format(SuccessMessage, seatingClass.Name));
            }

            context.SeatingClasses.AddRange(validSeatingClasses);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportTrains(StationsDbContext context, string jsonString)
        {
            TrainDto[] deserializedTrains = JsonConvert.DeserializeObject<TrainDto[]>(jsonString, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            List<Train> validTrains = new List<Train>();
            StringBuilder sb = new StringBuilder();

            foreach (var deserializedTrain in deserializedTrains)
            {
                bool trainAlreadyExists = validTrains.Any(t => t.TrainNumber == deserializedTrain.TrainNumber);

                bool seatsAreValid = deserializedTrain.Seats.All(IsValid);

                bool seatingClassesAreValid = deserializedTrain.Seats
                    .All(scd => context.SeatingClasses.Any(sc => sc.Name == scd.Name && sc.Abbreviation == scd.Abbreviation));

                if (!IsValid(deserializedTrain) || trainAlreadyExists || !seatsAreValid || !seatingClassesAreValid)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                TrainSeat[] trainSeats = deserializedTrain.Seats
                    .Select(s => new TrainSeat
                    {
                        SeatingClass = context.SeatingClasses.SingleOrDefault(sc => sc.Name == s.Name && sc.Abbreviation == s.Abbreviation),
                        Quantity = s.Quantity.Value,
                    }).ToArray();

                TrainType trainType = Enum.TryParse(deserializedTrain.Type, out TrainType type) ? type : TrainType.HighSpeed;

                Train train = new Train
                {
                    TrainNumber = deserializedTrain.TrainNumber,
                    Type = trainType,
                    TrainSeats = trainSeats
                };

                validTrains.Add(train);
                sb.AppendLine(string.Format(SuccessMessage, train.TrainNumber));
            }

            context.Trains.AddRange(validTrains);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportTrips(StationsDbContext context, string jsonString)
        {
            TripDto[] tripDtos = JsonConvert.DeserializeObject<TripDto[]>(jsonString, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });

            StringBuilder sb = new StringBuilder();
            List<Trip> validTrips = new List<Trip>();

            foreach (var tripDto in tripDtos)
            {
                if (!IsValid(tripDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                Train train = context.Trains.SingleOrDefault(t => t.TrainNumber == tripDto.Train);
                Station originStation = context.Stations.SingleOrDefault(s => s.Name == tripDto.OriginStation);
                Station destinationStation = context.Stations.SingleOrDefault(s => s.Name == tripDto.DestinationStation);

                if (train == null || originStation == null || destinationStation == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                DateTime departureTime = DateTime.ParseExact(tripDto.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                DateTime arrivalTime = DateTime.ParseExact(tripDto.ArrivalTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                if (departureTime > arrivalTime)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                TimeSpan timeDifference;
                if (tripDto.TimeDifference != null)
                {
                    timeDifference = TimeSpan.ParseExact(tripDto.TimeDifference, "hh\\:mm", CultureInfo.InvariantCulture);
                }

                TripStatus tripStatus = Enum.TryParse(tripDto.Status, out TripStatus status) ? status : TripStatus.OnTime;

                Trip trip = new Trip
                {
                    Train = train,
                    OriginStation = originStation,
                    DestinationStation = destinationStation,
                    DepartureTime = departureTime,
                    ArrivalTime = arrivalTime,
                    Status = status,
                    TimeDifference = timeDifference
                };

                validTrips.Add(trip);

                sb.AppendLine(string.Format(TripAddMessage, tripDto.OriginStation, tripDto.DestinationStation));
            }

            context.Trips.AddRange(validTrips);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportCards(StationsDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CardDto[]), new XmlRootAttribute("Cards"));

            CardDto[] cardDtos = (CardDto[])serializer.Deserialize(new StringReader(xmlString));

            List<CustomerCard> validCards = new List<CustomerCard>();
            StringBuilder sb = new StringBuilder();

            foreach (var cardDto in cardDtos)
            {
                if (!IsValid(cardDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                CardType cardType = Enum.TryParse(cardDto.CardType, out CardType type) ? type : CardType.Normal;

                CustomerCard customerCard = new CustomerCard
                {
                    Name = cardDto.Name,
                    Age = cardDto.Age,
                    Type = cardType,
                };

                validCards.Add(customerCard);
                sb.AppendLine(string.Format(SuccessMessage, customerCard.Name));
            }

            context.Cards.AddRange(validCards);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportTickets(StationsDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TicketDto[]), new XmlRootAttribute("Tickets"));

            TicketDto[] ticketDtos = (TicketDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder sb = new StringBuilder();
            List<Ticket> validTickets = new List<Ticket>();

            foreach (var ticketDto in ticketDtos)
            {
                if (!IsValid(ticketDto))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                DateTime tickedDepartureTime = DateTime.ParseExact(ticketDto.Trip.DepartureTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                Trip trip = context.Trips
                    .Include(t => t.OriginStation)
                    .Include(t => t.DestinationStation)
                    .Include(t => t.Train)
                    .ThenInclude(t => t.TrainSeats)
                    .SingleOrDefault(t => t.OriginStation.Name == ticketDto.Trip.OriginStationName &&
                                     t.DestinationStation.Name == ticketDto.Trip.DestinationStationName &&
                                     t.DepartureTime == tickedDepartureTime);

                if (trip == null)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                string seatingClassAbbreviation = ticketDto.Seat.Substring(0, 2);
                int seatNumber = int.Parse(ticketDto.Seat.Substring(2));

                TrainSeat trainSeat = trip.Train.TrainSeats.SingleOrDefault(s => s.SeatingClass.Abbreviation == seatingClassAbbreviation && s.Quantity >= seatNumber);

                CustomerCard customerCard = null;
                if (ticketDto.Card != null)
                {
                    customerCard = context.Cards.SingleOrDefault(c => c.Name == ticketDto.Card.Name);
                }

                if (trainSeat == null || customerCard == null || !IsValid(ticketDto.Card) || !IsValid(ticketDto.Trip))
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                Ticket ticket = new Ticket
                {
                    CustomerCard = customerCard,
                    Price = ticketDto.Price,
                    Trip = trip,
                    SeatingPlace = ticketDto.Seat
                };

                validTickets.Add(ticket);

                string departureTime = ticket.Trip.DepartureTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                sb.AppendLine(string.Format(TicketaAddMessage, ticket.Trip.OriginStation.Name, ticket.Trip.DestinationStation.Name, departureTime));
            }

            context.Tickets.AddRange(validTickets);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}