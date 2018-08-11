namespace FastFood.DataProcessor
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using FastFood.Data;
    using FastFood.Models;
    using FastFood.DataProcessor.Dto.Import;

    using Newtonsoft.Json;
    using System.Globalization;
    using AutoMapper;

    public static class Deserializer
    {
        private const string ERROR_MESSAGE = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";
        private const string AddedOrderMessage = "Order for {0} on {1} added";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            var employeeDtos = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<Employee>();

            foreach (var employeeDto in employeeDtos)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var employee = Mapper.Map<Employee>(employeeDto);
                var position = GetEmployeePosition(context, employeeDto.Position);
                employee.Position = position;

                validObjects.Add(employee);
                sb.AppendLine(string.Format(SuccessMessage, employee.Name));
            }

            context.Employees.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportItems(FastFoodDbContext context, string jsonString)
        {
            var itemDtos = JsonConvert.DeserializeObject<ItemDto[]>(jsonString);

            var sb = new StringBuilder();
            var validObjects = new List<Item>();

            foreach (var itemDto in itemDtos)
            {
                var alreadyExists = validObjects.Any(i => i.Name == itemDto.Name);
                if (!IsValid(itemDto) || alreadyExists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var item = Mapper.Map<Item>(itemDto);
                var category = GetItemCategory(context, itemDto.Category);
                item.Category = category;

                validObjects.Add(item);
                sb.AppendLine(string.Format(SuccessMessage, item.Name));
            }

            context.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(OrderDto[]), new XmlRootAttribute("Orders"));
            var orderDtos = (OrderDto[])serializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var validObjects = new List<Order>();

            foreach (var orderDto in orderDtos)
            {
                var orderItemsAreValid = orderDto.Items.All(IsValid);

                if (!IsValid(orderDto) || !orderItemsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var employee = context.Employees.SingleOrDefault(e => e.Name == orderDto.Employee);
                if (employee == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var orderItemNames = orderDto.Items
                    .Select(i => i.Name)
                    .ToArray();

                var validItemNames = context.Items
                    .Where(i => orderItemNames.Contains(i.Name))
                    .Select(i => i.Name)
                    .ToArray();

                if (orderItemNames.Length < validItemNames.Length)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var order = Mapper.Map<Order>(orderDto);

                var orderItems = new List<OrderItem>();
                foreach (var orderItemDto in orderDto.Items)
                {
                    var item = context.Items.SingleOrDefault(i => i.Name == orderItemDto.Name);
                    var orderItem = new OrderItem
                    {
                        Item = item,
                        Quantity = orderItemDto.Quantity,
                        Order = order,
                    };
                    orderItems.Add(orderItem);
                }

                order.Employee = employee;
                order.OrderItems = orderItems;

                validObjects.Add(order);
                sb.AppendLine($"Order for {order.Customer} on {order.DateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} added");
            }

            context.AddRange(validObjects);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static Position GetEmployeePosition(FastFoodDbContext context, string positinoName)
        {
            var position = context.Positions.SingleOrDefault(p => p.Name == positinoName);

            if (position == null)
            {
                position = new Position
                {
                    Name = positinoName,
                };

                context.Positions.Add(position);
                context.SaveChanges();
            }

            return position;
        }

        private static Category GetItemCategory(FastFoodDbContext context, string categoryName)
        {
            var category = context.Categories.SingleOrDefault(c => c.Name == categoryName);

            if (category == null)
            {

                category = new Category
                {
                    Name = categoryName,
                };

                context.Categories.Add(category);
                context.SaveChanges();
            }

            return category;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}