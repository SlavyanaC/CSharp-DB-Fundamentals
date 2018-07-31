namespace ProductsShop.App.Dtos.Json.Export
{
    using Newtonsoft.Json;

    public class UserDto
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("age")]
        public string Age { get; set; }

        [JsonProperty("soldProducts")]
        public SoldProductsCollectionDto SoldProducts { get; set; }
    }
}
