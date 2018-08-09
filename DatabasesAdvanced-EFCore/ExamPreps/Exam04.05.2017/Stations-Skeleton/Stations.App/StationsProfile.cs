namespace Stations.App
{
    using AutoMapper;
    using Stations.DataProcessor.Dto.Import;
    using Stations.DataProcessor.Dto.Import.Ticket;
    using Stations.Models;

    public class StationsProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public StationsProfile()
        {
            CreateMap<StationDto, Station>();

            CreateMap<SeatingClassDto, SeatingClass>();

            CreateMap<CardDto, CustomerCard>();

            CreateMap<TicketDto, Ticket>()
                .ForMember(dest => dest.Trip, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerCard, opt => opt.Ignore());
        }
    }
}
