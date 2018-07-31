namespace ProductsShop.App.Dtos.Json.Export
{
    using Newtonsoft.Json;

    [JsonObject]
    public class SoldProductNamePriceDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
