namespace Stations.App
{
    using Stations.Models;
    using System.Linq;
    using System.Globalization;

    using AutoMapper;

    using Export = Stations.DataProcessor.Dto.Export;
    using Import = Stations.DataProcessor.Dto.Import;
    using Stations.DataProcessor.Dto.Import.Ticket;

    public class StationsProfile : Profile
    {
        private const string DateTimeFormat = "dd/MM/yyyy hh:mm";

        public StationsProfile()
        {
            CreateMap<Import.StationDto, Station>();

            CreateMap<Import.SeatingClassDto, SeatingClass>();

            CreateMap<Import.CardDto, CustomerCard>();

            CreateMap<TicketDto, Ticket>()
                .ForMember(dest => dest.Trip, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerCard, opt => opt.Ignore());

            CreateMap<Train, Export.DelayedTrainDto>()
                .ForMember(dest => dest.DelayedTimes,
                           opt => opt.MapFrom(t => t.Trips
                                .Where(tr => tr.Status == Models.Enums.TripStatus.Delayed).Count()))
                .ForMember(dest => dest.MaxDelayedTime,
                           opt => opt.MapFrom(t => t.Trips.Max(tr => tr.TimeDifference)));

            CreateMap<Ticket, Export.TicketDto>()
                .ForMember(dest => dest.OriginStation, opt => opt.MapFrom(t => t.Trip.OriginStation.Name))
                .ForMember(dest => dest.DestinationStation, opt => opt.MapFrom(t => t.Trip.DestinationStation.Name))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(t => t.Trip.DepartureTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CustomerCard, Export.CardDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(c => c.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(c => c.Type))
                .ForMember(dest => dest.Tickets, opt => opt.MapFrom(c => c.BoughtTickets));
        }
    }
}
