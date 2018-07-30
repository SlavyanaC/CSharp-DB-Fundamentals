﻿namespace ProductsShop.App
{
    using AutoMapper;
    using ProductsShop.App.Dtos.Import;
    using ProductsShop.Models;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserImportDto, User>();

            CreateMap<ProductImportDto, Product>();

            CreateMap<CategoryImportDto, Category>();
        }
    }
}
