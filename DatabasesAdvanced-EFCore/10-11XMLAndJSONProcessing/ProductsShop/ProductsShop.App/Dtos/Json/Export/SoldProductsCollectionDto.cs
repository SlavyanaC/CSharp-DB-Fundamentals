namespace ProductsShop.App.Dtos.Json.Export
{
    using Newtonsoft.Json;

    public class SoldProductsCollectionDto
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("products")]
        public SoldProductNamePriceDto[] SoldProduct { get; set; }
    }
}
