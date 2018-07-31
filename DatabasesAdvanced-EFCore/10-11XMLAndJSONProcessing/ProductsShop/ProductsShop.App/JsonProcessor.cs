namespace ProductsShop.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DataAnnotations = System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;
    using ProductsShop.App.Dtos.Json.Export;
    using ProductsShop.Data;
    using ProductsShop.Models;

    public class JsonProcessor
    {
        private const string pathToJsonsFolder = "../../../Jsons/";

        public JsonProcessor()
        {
            this.Context = new ProductsShopContext();
        }

        public ProductsShopContext Context { get; private set; }

        public void ImportData()
        {
            ImportUsers();

            ImportProducts();

            ImportCategories();

            GenerateCategories();
        }

        private void ImportUsers()
        {
            string file = File.ReadAllText(pathToJsonsFolder + "users.json");

            User[] users = JsonConvert.DeserializeObject<User[]>(file);

            this.Context.Users.AddRange(users);
            this.Context.SaveChanges();
        }

        private void ImportProducts()
        {
            string file = File.ReadAllText(pathToJsonsFolder + "products.json");
            Product[] deserializedProducts = JsonConvert.DeserializeObject<Product[]>(file);

            Random random = new Random();
            int randomMax = this.Context.Users.Count() + 1;

            List<Product> products = new List<Product>();
            int counter = 1;
            foreach (var product in deserializedProducts)
            {
                int sellerId = random.Next(1, randomMax / 2);
                int buyerId = random.Next((randomMax / 2) + 1, randomMax);

                product.SellerId = sellerId;

                if (counter == 4)
                {
                    product.BuyerId = null;
                    counter = 0;
                }
                else
                {
                    product.BuyerId = buyerId;
                }
                counter++;

                products.Add(product);
            }

            this.Context.Products.AddRange(products);
            this.Context.SaveChanges();
        }

        private void ImportCategories()
        {
            string file = File.ReadAllText(pathToJsonsFolder + "categories.json");

            Category[] categories = JsonConvert.DeserializeObject<Category[]>(file);

            this.Context.Categories.AddRange(categories);
            this.Context.SaveChanges();
        }

        private void GenerateCategories()
        {
            Random random = new Random();

            int productsCount = this.Context.Products.Count();
            int categoriesMaxRandom = this.Context.Categories.Count() + 1;

            List<CategoryProducts> categoryProducts = new List<CategoryProducts>();
            for (int productId = 1; productId <= productsCount; productId++)
            {
                int categoryId = random.Next(1, categoriesMaxRandom);

                CategoryProducts categoryProduct = new CategoryProducts
                {
                    CategoryId = categoryId,
                    ProductId = productId,
                };

                categoryProducts.Add(categoryProduct);
            }

            this.Context.CategoryProducts.AddRange(categoryProducts);
            this.Context.SaveChanges();
        }

        public void ExportProductsInRange()
        {
            ProductInRangeDto[] productsInRange = this.Context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ProductInRangeDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    SellerFullName = $"{p.Seller.FirstName} {p.Seller.LastName}" ?? p.Seller.LastName,
                })
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };

            string jsonString = JsonConvert.SerializeObject(productsInRange, Formatting.Indented, settings);

            File.WriteAllText(pathToJsonsFolder + "products-in-range.json", jsonString);
        }

        public void ExportSoldProducts()
        {
            UserWithSoldProductsDto[] usersWithSoldProducts = this.Context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserWithSoldProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new SoldProductDto
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName,
                    }).ToArray(),
                })
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };

            string jsonString = JsonConvert.SerializeObject(usersWithSoldProducts, Formatting.Indented, settings);

            File.WriteAllText(pathToJsonsFolder + "users-sold-products.json", jsonString);
        }

        public void ExportCategoriesByProductsCount()
        {
            CategoryByProductDto[] categoriesByProducts = this.Context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryByProductDto
                {
                    CategoryName = c.Name,
                    ProductsCount = c.CategoryProducts.Count,
                    AvgPrice = c.CategoryProducts.Select(p => p.Product.Price).DefaultIfEmpty(0).Average(),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price),
                })
                .ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };

            string jsonString = JsonConvert.SerializeObject(categoriesByProducts, Formatting.Indented, settings);

            File.WriteAllText(pathToJsonsFolder + "categories-by-products.json", jsonString);
        }

        public void ExportUsersAndProducts()
        {
            UsersColletionDto usersColletion = new UsersColletionDto
            {
                Count = this.Context.Users.Count(),
                Users = this.Context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .Select(u => new UserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age.ToString(),
                    SoldProducts = new SoldProductsCollectionDto
                    {
                        Count = u.ProductsSold.Count,
                        SoldProduct = u.ProductsSold.Select(p => new SoldProductNamePriceDto
                        {
                            Name = p.Name,
                            Price = p.Price,
                        }).ToArray()
                    }
                }).ToArray()
            };

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };

            string jsonString = JsonConvert.SerializeObject(usersColletion, Formatting.Indented, settings);

            File.WriteAllText(pathToJsonsFolder + "users-and-products.json", jsonString);
        }

        private bool IsValid(object obj)
        {
            var validatonContext = new DataAnnotations.ValidationContext(obj);
            var validationResults = new List<DataAnnotations.ValidationResult>();

            return DataAnnotations.Validator.TryValidateObject(obj, validatonContext, validationResults, true);
        }
    }
}