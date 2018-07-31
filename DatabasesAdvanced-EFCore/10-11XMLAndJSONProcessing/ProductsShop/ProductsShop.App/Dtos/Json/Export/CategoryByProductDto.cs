namespace ProductsShop.App.Dtos.Json.Export
{
    using Newtonsoft.Json;

    public class CategoryByProductDto
    {
        [JsonProperty("name")]
        public string CategoryName { get; set; }

        [JsonProperty("productsCount")]
        public int ProductsCount { get; set; }

        [JsonProperty("averagePrice")]
        public decimal AvgPrice { get; set; }

        [JsonProperty("totalRevenue")]
        public decimal TotalRevenue { get; set; }
    }
}
