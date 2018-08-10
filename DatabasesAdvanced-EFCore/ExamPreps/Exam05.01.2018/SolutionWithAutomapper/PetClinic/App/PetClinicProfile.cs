namespace PetClinic.App
{
    using System;
    using System.Globalization;
    using AutoMapper;

    using Models;
    using Export = DataProcessor.Dto.Export;
    using Import = DataProcessor.Dto.Import;

    public class PetClinicProfile : Profile
    {
        public PetClinicProfile()
        {
            CreateMap<Import.AnimalAidDto, AnimalAid>();

            CreateMap<Import.PassportDto, Passport>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(x => DateTime.ParseExact(x.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            CreateMap<Import.AnimalDto, Animal>();

            CreateMap<Import.VetDto, Vet>();

            CreateMap<Animal, Export.AnimalDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(x => x.Passport.OwnerName))
                .ForMember(dest => dest.AnimalName, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(x => x.PassportSerialNumber))
                .ForMember(dest => dest.RegisteredOn, opt => opt.MapFrom(x => x.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)));

            CreateMap<ProcedureAnimalAid, Export.AnimalAidDto>()
                .ForMember(d => d.Name, o => o.MapFrom(x => x.AnimalAid.Name))
                .ForMember(d => d.Price, o => o.MapFrom(x => x.AnimalAid.Price))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Procedure, Export.ProcedureDto>()
                .ForMember(dest => dest.Passport, opt => opt.MapFrom(x => x.Animal.Passport.SerialNumber))
                .ForMember(dest => dest.OwnerNumber, opt => opt.MapFrom(x => x.Animal.Passport.OwnerPhoneNumber))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(x => x.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.AnimalAids, opt => opt.MapFrom(x => x.ProcedureAnimalAids))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(x => x.Cost));
        }
    }
}
