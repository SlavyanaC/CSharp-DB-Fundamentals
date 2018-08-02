namespace FastFood.DataProcessor
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Linq;
    using System.Xml.Serialization;

    using Json = Newtonsoft.Json;

    using FastFood.Data;
    using FastFood.Models.Enums;
    using FastFood.DataProcessor.Dto.Export;

    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var type = Enum.Parse<OrderType>(orderType);

            var employee = context.Employees
                .Where(e => e.Name == employeeName)
                .Select(e => new
                {
                    e.Name,
                    Orders = e.Orders.Where(o => o.Type == type)
                        .Select(o => new
                        {
                            o.Customer,
                            Items = o.OrderItems.Select(oi => new
                            {
                                oi.Item.Name,
                                oi.Item.Price,
                                oi.Quantity
                            }).ToArray(),
                            TotalPrice = o.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity)
                        })
                        .OrderByDescending(o => o.TotalPrice)
                        .ThenByDescending(o => o.Items.Length)
                        .ToArray(),
                    TotalMade = e.Orders.Where(o => o.Type == type)
                        .Sum(o => o.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity))
                })
                .SingleOrDefault();

            var jsonsString = Json.JsonConvert.SerializeObject(employee, Json.Formatting.Indented);

            return jsonsString;
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            string[] categories = categoriesString.Split(',');

            CategoryDto[] categoryDtos = context.Items
                .Where(i => categories.Contains(i.Category.Name))
                .GroupBy(i => i.Category.Name)
                .Select(gr => new CategoryDto
                {
                    Name = gr.Key,
                    MostPopularItem = gr.Select(i => new MostPopularItemDto
                    {
                        Name = i.Name,
                        TotalMade = i.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity),
                        TimesSold = i.OrderItems.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(g => g.TotalMade)
                    .ThenByDescending(g => g.TimesSold)
                    .FirstOrDefault(),
                })
                .OrderByDescending(g => g.MostPopularItem.TotalMade)
                .ThenByDescending(g => g.MostPopularItem.TimesSold)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(CategoryDto[]), new XmlRootAttribute("Categories"));

            serializer.Serialize(new StringWriter(sb), categoryDtos, namespaces);

            return sb.ToString();
        }
    }
}