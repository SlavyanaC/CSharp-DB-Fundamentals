namespace CarDealer.App
{
    using AutoMapper;

    using CarDealer.App.Dtos.Import;
    using CarDealer.Models;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SupplierDto, Supplier>();

            CreateMap<PartDto, Part>();

            CreateMap<CarDto, Car>();

            CreateMap<CustomerDto, Customer>();
        }
    }
}
