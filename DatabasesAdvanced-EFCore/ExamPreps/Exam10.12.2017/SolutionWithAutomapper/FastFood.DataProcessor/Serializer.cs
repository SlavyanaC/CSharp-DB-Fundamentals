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
    using AutoMapper.QueryableExtensions;

    public class Serializer
    {
        public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var type = Enum.Parse<OrderType>(orderType);

            var employee = context.Employees
                .Where(e => e.Name == employeeName && e.Orders.All(o => o.Type == type))
                .ProjectTo<EmployeeDto>()
                .SingleOrDefault();

            var jsonsString = Json.JsonConvert.SerializeObject(employee, Json.Formatting.Indented);

            return jsonsString;
        }

        public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categories = categoriesString.Split(',').ToArray();

            var categoryDtos = context.Items
                .Where(i => categories.Any(c => c == i.Category.Name))
                .GroupBy(i => i.Category.Name)
                .Select(gr => new CategoryDto
                {
                    Name = gr.Key,
                    MostPopularItem = gr.Select(i => new MostPopularItemDto
                    {
                        Name = i.Name,
                        TotalMade = i.OrderItems.Sum(oi => oi.Item.Price * oi.Quantity),
                        TimesSold = i.OrderItems.Sum(oi => oi.Quantity),
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