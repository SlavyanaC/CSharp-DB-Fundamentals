namespace FastFood.DataProcessor.Dto.Export
{
    public class OrderDto
    {
        public string Customer { get; set; }

        public ItemDto[] Items { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
