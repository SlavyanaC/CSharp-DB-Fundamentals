namespace ProductsShop.App
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using DataAnnotations = System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using ProductsShop.App.Dtos.Import;
    using ProductsShop.Models;
    using ProductsShop.Data;
    using ProductsShop.App.Dtos.Export;

    class StartUp
    {
        public static void Main(string[] args)
        {
            ImportData();

            ExportProductsInRange();

            ExportSoldProducts();

            ExportCategoriesByProductsCount();

            ExportUsersAndProducts();
        }

        private static void ExportUsersAndProducts()
        {
            ProductsShopContext context = new ProductsShopContext();

            UsersColletionDto users = new UsersColletionDto
            {
                Count = context.Users.Count(),
                Users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .Select(u => new UserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age.ToString(),
                    SoldProduct = new SoldProductsCollectionDto
                    {
                        Count = u.ProductsSold.Count(),
                        SoldProducts = u.ProductsSold.Select(p => new SoldProductsAttributesDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).ToArray()
                    }
                }).ToArray()
            };

            StringBuilder builder = new StringBuilder();
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            XmlSerializer serializer = new XmlSerializer(typeof(UsersColletionDto), new XmlRootAttribute("users"));
            serializer.Serialize(new StringWriter(builder), users, xmlNamespaces);

            File.WriteAllText("../../../Xmls/users-and-products.xml", builder.ToString());
        }

        private static void ExportCategoriesByProductsCount()
        {
            ProductsShopContext context = new ProductsShopContext();

            CategoryByProductDto[] categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new CategoryByProductDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Select(cp => cp.Product.Price).DefaultIfEmpty(0).Average(),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price),
                })
                .ToArray();

            StringBuilder builder = new StringBuilder();
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            XmlSerializer serializer = new XmlSerializer(typeof(CategoryByProductDto[]), new XmlRootAttribute("categories"));
            serializer.Serialize(new StringWriter(builder), categories, xmlNamespaces);

            File.WriteAllText("../../../Xmls/categories-by-products.xml", builder.ToString());
        }

        private static void ExportSoldProducts()
        {
            ProductsShopContext context = new ProductsShopContext();

            UserSoldProductsDto[] users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserSoldProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(s => new SoldProductDto
                    {
                        Name = s.Name,
                        Price = s.Price,
                    })
                    .ToArray()
                })
                .ToArray();

            StringBuilder builder = new StringBuilder();
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            XmlSerializer serializer = new XmlSerializer(typeof(UserSoldProductsDto[]), new XmlRootAttribute("users"));
            serializer.Serialize(new StringWriter(builder), users, xmlNamespaces);

            File.WriteAllText("../../../Xmls/users-sold-products.xml", builder.ToString());

        }

        private static void ExportProductsInRange()
        {
            ProductsShopContext context = new ProductsShopContext();

            ProductInRangeDto[] products = context.Products
                .Where(p => p.Price >= 1000 && p.Price <= 2000 && p.Buyer != null)
                .OrderByDescending(p => p.Price)
                .Select(p => new ProductInRangeDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    BueyerNames = $"{p.Buyer.FirstName} {p.Buyer.LastName}" ?? p.Buyer.LastName,
                })
                .ToArray();

            StringBuilder builder = new StringBuilder();

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(ProductInRangeDto[]), new XmlRootAttribute("products"));

            serializer.Serialize(new StringWriter(builder), products, xmlNamespaces);

            File.WriteAllText("../../../Xmls/products-in-range.xml", builder.ToString());
        }

        private static void ImportData()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            IMapper mapper = config.CreateMapper();

            ImportUsers(mapper);
            ImportProducts(mapper);
            ImportCategories(mapper);
            GenereteCategoryProducts();
        }

        private static void GenereteCategoryProducts()
        {
            Random random = new Random();
            List<CategoryProducts> categoryProducts = new List<CategoryProducts>();

            for (int productId = 1; productId < 200; productId++)
            {
                int categoryId = random.Next(1, 12);

                CategoryProducts categoryProduct = new CategoryProducts()
                {
                    ProductId = productId,
                    CategoryId = categoryId,
                };

                categoryProducts.Add(categoryProduct);
            }

            ProductsShopContext context = new ProductsShopContext();
            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();
        }

        private static void ImportCategories(IMapper mapper)
        {
            string xmlString = File.ReadAllText("../../../Xmls/categories.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(CategoryImportDto[]), new XmlRootAttribute("categories"));
            CategoryImportDto[] deserializedCategories = (CategoryImportDto[])serializer.Deserialize(new StringReader(xmlString));

            List<Category> categories = new List<Category>();

            foreach (var categoryDto in deserializedCategories)
            {
                if (!IsValid(categoryDto))
                {
                    continue;
                }

                Category category = mapper.Map<Category>(categoryDto);
                categories.Add(category);
            }

            ProductsShopContext context = new ProductsShopContext();
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        private static void ImportProducts(IMapper mapper)
        {
            string xmlString = File.ReadAllText("../../../Xmls/products.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(ProductImportDto[]), new XmlRootAttribute("products"));
            ProductImportDto[] deserializedProducts = (ProductImportDto[])serializer.Deserialize(new StringReader(xmlString));

            Random random = new Random();
            List<Product> products = new List<Product>();

            int counter = 1;

            foreach (var productDto in deserializedProducts)
            {
                if (!IsValid(productDto))
                {
                    continue;
                }

                Product product = mapper.Map<Product>(productDto);

                int buyerId = random.Next(1, 30);
                int sellerId = random.Next(31, 57);

                product.BuyerId = buyerId;
                product.SellerId = sellerId;

                if (counter == 4)
                {
                    product.BuyerId = null;
                    counter = 0;
                }

                products.Add(product);

                counter++;
            }

            ProductsShopContext context = new ProductsShopContext();
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        private static void ImportUsers(IMapper mapper)
        {
            string xmlString = File.ReadAllText("../../../Xmls/users.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(UserImportDto[]), new XmlRootAttribute("users"));
            UserImportDto[] deserializedUsers = (UserImportDto[])serializer.Deserialize(new StringReader(xmlString));

            List<User> users = new List<User>();

            foreach (var userDto in deserializedUsers)
            {
                if (!IsValid(userDto))
                {
                    continue;
                }

                User user = mapper.Map<User>(userDto);
                users.Add(user);
            }

            ProductsShopContext context = new ProductsShopContext();
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        public static bool IsValid(object obj)
        {
            var validatonContext = new DataAnnotations.ValidationContext(obj);
            var validationResults = new List<DataAnnotations.ValidationResult>();

            return DataAnnotations.Validator.TryValidateObject(obj, validatonContext, validationResults, true);
        }
    }
}
