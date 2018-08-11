namespace FastFood.DataProcessor
{
    using System;
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
    using FastFood.Models.Enums;

    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";
        private const string AddedOrderMessage = "Order for {0} on {1} added";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            EmployeeDto[] employeeDtos = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);
            List<Employee> validEmployees = new List<Employee>();

            StringBuilder result = new StringBuilder();
            foreach (var employeeDto in employeeDtos)
            {
                if (!IsValid(employeeDto))
                {
                    result.AppendLine(FailureMessage);
                    continue;
                }

                Position position = GetEmployeePosition(context, employeeDto.Position);

                Employee employee = new Employee
                {
                    Name = employeeDto.Name,
                    Age = employeeDto.Age,
                    Position = position
                };

                validEmployees.Add(employee);
                result.AppendLine(string.Format(SuccessMessage, employee.Name));
            }

            context.Employees.AddRange(validEmployees);
            context.SaveChanges();

            return result.ToString().Trim();
        }

        public static string ImportItems(FastFoodDbContext context, string jsonString)
        {
            ItemDto[] employeeDtos = JsonConvert.DeserializeObject<ItemDto[]>(jsonString);
            List<Item> validItems = new List<Item>();

            StringBuilder result = new StringBuilder();
            foreach (var itemDto in employeeDtos)
            {
                if (!IsValid(itemDto) || validItems.Any(i => i.Name == itemDto.Name))
                {
                    result.AppendLine(FailureMessage);
                    continue;
                }

                Category category = GetItemCategory(context, itemDto.Category);

                Item item = new Item
                {
                    Name = itemDto.Name,
                    Price = itemDto.Price,
                    Category = category
                };

                validItems.Add(item);
                result.AppendLine(string.Format(SuccessMessage, item.Name));
            }

            context.Items.AddRange(validItems);
            context.SaveChanges();

            return result.ToString().Trim();
        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OrderDto[]), new XmlRootAttribute("Orders"));

            OrderDto[] orderDtos = (OrderDto[])serializer.Deserialize(new StringReader(xmlString));

            List<Order> validOrders = new List<Order>();
            StringBuilder result = new StringBuilder();

            foreach (var orderDto in orderDtos)
            {
                Employee orderEmployee = GetEmployee(context, orderDto.Employee);

                bool itemsAreValid = orderDto.Items.All(IsValid);
                bool orderItemsExist = orderDto.Items.All(i => context.Items.Any(ci => ci.Name == i.Name));

                if (!IsValid(orderDto) || !itemsAreValid || orderEmployee == null || !orderItemsExist)
                {
                    result.AppendLine(FailureMessage);
                    continue;
                }

                List<OrderItem> orderItems = new List<OrderItem>();

                foreach (var orderItemDto in orderDto.Items)
                {
                    Item item = GetItem(context, orderItemDto.Name);

                    OrderItem orderItem = new OrderItem
                    {
                        Item = item,
                        Quantity = orderItemDto.Quantity
                    };

                    orderItems.Add(orderItem);
                }

                Order order = new Order
                {
                    Customer = orderDto.Customer,
                    Employee = orderEmployee,
                    DateTime = DateTime.ParseExact(orderDto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Type = Enum.Parse<OrderType>(orderDto.Type),
                    OrderItems = orderItems,
                };

                validOrders.Add(order);
                result.AppendLine(string.Format(AddedOrderMessage, orderDto.Customer, orderDto.DateTime));
            }

            context.Orders.AddRange(validOrders);
            context.SaveChanges();

            return result.ToString().Trim();
        }

        private static Position GetEmployeePosition(FastFoodDbContext context, string positinoName)
        {
            Position position = context.Positions.SingleOrDefault(p => p.Name == positinoName);

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
            Category category = context.Categories.SingleOrDefault(c => c.Name == categoryName);

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

        private static Item GetItem(FastFoodDbContext context, string name)
        {
            Item item = context.Items.SingleOrDefault(i => i.Name == name);
            return item;
        }

        private static Employee GetEmployee(FastFoodDbContext context, string employeeName)
        {
            Employee employee = context.Employees.SingleOrDefault(e => e.Name == employeeName);
            return employee;
        }

        private static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}