namespace SoftJail
{
    using System;
    using System.Globalization;

    using AutoMapper;

    using SoftJail.Data.Models;
    using Import = SoftJail.DataProcessor.ImportDto;
    using Export = SoftJail.DataProcessor.ExportDto;
    using System.Linq;

    public class SoftJailProfile : Profile
    {
        public SoftJailProfile()
        {
            CreateMap<Import.CellDto, Cell>();

            CreateMap<Import.DepartmentDto, Department>();

            CreateMap<Import.MailDto, Mail>();

            CreateMap<Import.PrisonerDto, Prisoner>()
                .ForMember(d => d.Bail, o => o.Ignore())
                .ForMember(d => d.ReleaseDate, o => o.Ignore())
                .ForMember(d => d.IncarcerationDate, o => o.MapFrom(x => DateTime.ParseExact(x.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)));

            CreateMap<Import.OfficerDto, Officer>()
                .ForMember(d => d.FullName, o => o.MapFrom(x => x.Name));

            CreateMap<OfficerPrisoner, Export.OfficerDto>()
                .ForMember(d => d.Department, o => o.MapFrom(x => x.Officer.Department.Name))
                .ForMember(d => d.OfficerName, o => o.MapFrom(x => x.Officer.FullName));

            CreateMap<Prisoner, Export.PrisonerDto>()
                .ForMember(d => d.Name, o => o.MapFrom(x => x.FullName))
                .ForMember(d => d.CellNumber, o => o.MapFrom(x => x.Cell.CellNumber))
                .ForMember(d => d.Officers, o => o.MapFrom(x => x.PrisonerOfficers.OrderBy(y => y.Officer.FullName)))
                .ForMember(d => d.TotalOfficerSalary, o => o.MapFrom(x => x.PrisonerOfficers.Sum(y => y.Officer.Salary)));

            Func<String, String> reverse = null;
            reverse = s => s.Length == 1 ? s : reverse(s.Substring(1)) + s[0];

            CreateMap<Mail, Export.MessageDto>()
                .ForMember(d => d.Description, o => o.MapFrom(x => reverse(x.Description)));

            CreateMap<Prisoner, Export.PrisonerMailDto>()
                .ForMember(d => d.Name, o => o.MapFrom(x => x.FullName))
                .ForMember(d => d.IncarcerationDate, o => o.MapFrom(x => x.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)))
                .ForMember(d => d.EncryptedMessages, o => o.MapFrom(x => x.Mails));
        }
    }
}
