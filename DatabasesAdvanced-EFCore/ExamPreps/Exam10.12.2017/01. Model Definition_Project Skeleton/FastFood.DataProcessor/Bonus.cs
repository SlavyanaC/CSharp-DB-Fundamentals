namespace FastFood.DataProcessor
{
    using System;
    using System.Linq;
    using FastFood.Data;
    using FastFood.Models;

    public static class Bonus
    {
        private const string FailureMessage = "Item {0} not found!";

        public static string UpdatePrice(FastFoodDbContext context, string itemName, decimal newPrice)
        {
            Item item = context.Items.SingleOrDefault(i => i.Name == itemName);

            if (item == null)
            {
                return string.Format(FailureMessage, itemName);
            }

            decimal oldPrice = item.Price;
            item.Price = newPrice;
            context.SaveChanges();

            var result = $"{itemName} Price updated from ${oldPrice:F2} to ${newPrice:F2}";
            return result;
        }
    }
}
