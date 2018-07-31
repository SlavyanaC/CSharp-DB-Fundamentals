namespace ProductsShop.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Xml;

    using AutoMapper;
    using DataAnnotations = System.ComponentModel.DataAnnotations;

    using ProductsShop.App.Dtos.Xml.Export;
    using ProductsShop.App.Dtos.Xml.Import;
    using ProductsShop.Data;
    using ProductsShop.Models;

    public class XmlProcessor
    {
        private const string pathToXmlsFolder = "../../../Xmls/";

        public XmlProcessor()
        {
            this.Context = new ProductsShopContext();
        }

        public ProductsShopContext Context { get; set; }

        public void ImportData()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            IMapper mapper = config.CreateMapper();

            this.ImportUsers(mapper);
            this.ImportProducts(mapper);
            this.ImportCategories(mapper);
            this.GenerateCategoryProducts();
        }

        private void ImportUsers(IMapper mapper)
        {
            string xmlString = File.ReadAllText(pathToXmlsFolder + "users.xml");

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

            this.Context.Users.AddRange(users);
            this.Context.SaveChanges();
        }

        private void ImportProducts(IMapper mapper)
        {
            string xmlString = File.ReadAllText(pathToXmlsFolder + "products.xml");

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

                int sellerId = random.Next(31, 57);
                int buyerId = random.Next(1, 30);

                product.SellerId = sellerId;
                product.BuyerId = buyerId;

                if (counter == 4)
                {
                    product.BuyerId = null;
                    counter = 0;
                }

                products.Add(product);

                counter++;
            }

            this.Context.Products.AddRange(products);
            this.Context.SaveChanges();
        }

        private void ImportCategories(IMapper mapper)
        {
            string xmlString = File.ReadAllText(pathToXmlsFolder + "categories.xml");

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

            this.Context.Categories.AddRange(categories);
            this.Context.SaveChanges();
        }

        private void GenerateCategoryProducts()
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

            this.Context.CategoryProducts.AddRange(categoryProducts);
            this.Context.SaveChanges();
        }

        private bool IsValid(object obj)
        {
            var validatonContext = new DataAnnotations.ValidationContext(obj);
            var validationResults = new List<DataAnnotations.ValidationResult>();

            return DataAnnotations.Validator.TryValidateObject(obj, validatonContext, validationResults, true);
        }

        public void ExportProductsInRange()
        {
            ProductInRangeDto[] products = this.Context.Products
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

            File.WriteAllText(pathToXmlsFolder + "products-in-range.xml", builder.ToString());
        }

        public void ExportSoldProducts()
        {
            UserSoldProductsDto[] users = this.Context.Users
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

            File.WriteAllText(pathToXmlsFolder + "users-sold-products.xml", builder.ToString());
        }

        public void ExportCategoriesByProductsCount()
        {
            CategoryByProductDto[] categories = this.Context.Categories
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

            File.WriteAllText(pathToXmlsFolder + "categories-by-products.xml", builder.ToString());
        }

        public void ExportUsersAndProducts()
        {
            UsersColletionDto users = new UsersColletionDto
            {
                Count = this.Context.Users.Count(),
                Users = this.Context.Users
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
    }
}
