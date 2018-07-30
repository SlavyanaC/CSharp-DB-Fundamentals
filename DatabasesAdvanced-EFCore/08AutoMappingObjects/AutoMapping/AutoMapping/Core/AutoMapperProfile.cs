namespace AutoMapping.Core
{
    using AutoMapper;
    using AutoMapping.Data.Models;
    using AutoMapping.Employees.DTOModels;

    class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Employee, EmployeeDTO>().ReverseMap();
            CreateMap<Employee, EmployeePersonalDTO>().ReverseMap();
            CreateMap<Employee, ManagerDTO>()
                .ForMember(dest => dest.EmployeeDTOs, 
                           from => from.MapFrom(e => e.ManagerEmployees))
                .ReverseMap();
        }
    }
}
