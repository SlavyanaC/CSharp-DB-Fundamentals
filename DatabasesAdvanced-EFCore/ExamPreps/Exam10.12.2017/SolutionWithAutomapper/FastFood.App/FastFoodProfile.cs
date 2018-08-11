namespace FastFood.App
{
    using System;
    using System.Linq;
    using System.Globalization;

    using AutoMapper;

    using FastFood.Models;
    using FastFood.Models.Enums;
    using Import = FastFood.DataProcessor.Dto.Import;
    using Export = FastFood.DataProcessor.Dto.Export;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            CreateMap<Import.EmployeeDto, Employee>()
                .ForMember(d => d.Name, o => o.MapFrom(e => e.Name))
                .ForMember(d => d.Age, o => o.MapFrom(e => e.Age))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Import.ItemDto, Item>()
                .ForMember(d => d.Name, o => o.MapFrom(i => i.Name))
                .ForMember(d => d.Price, o => o.MapFrom(i => i.Price))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Import.OrderDto, Order>()
                .ForMember(d => d.DateTime, o => o.MapFrom(x => DateTime.ParseExact(x.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)))
                .ForMember(d => d.Customer, o => o.MapFrom(x => x.Customer))
                .ForMember(d => d.Type, o => o.MapFrom(x => Enum.Parse<OrderType>(x.Type)))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<OrderItem, Export.ItemDto>()
                .ForMember(d => d.Name, o => o.MapFrom(x => x.Item.Name))
                .ForMember(d => d.Price, o => o.MapFrom(x => x.Item.Price));

            CreateMap<Order, Export.OrderDto>()
                .ForMember(d => d.Items, o => o.MapFrom(x => x.OrderItems));

            CreateMap<Employee, Export.EmployeeDto>()
                .ForMember(d => d.TotalMade, o => o.MapFrom(x => x.Orders.Sum(or => or.TotalPrice)))
                .ForMember(d => d.Orders, o => o.MapFrom(x => x.Orders.OrderByDescending(y => y.TotalPrice)));
        }
    }
}
