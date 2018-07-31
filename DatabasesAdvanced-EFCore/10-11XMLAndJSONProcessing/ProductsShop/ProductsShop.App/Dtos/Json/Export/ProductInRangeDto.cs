namespace ProductsShop.App.Dtos.Json.Export
{
    using Newtonsoft.Json;

    public class ProductInRangeDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("seller")]
        public string SellerFullName { get; set; }
    }
}
