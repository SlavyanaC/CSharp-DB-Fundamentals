namespace ProductsShop.App.Dtos.Json.Export
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class UserWithSoldProductsDto
    {
        [JsonProperty("firsName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("soldProducts")]
        public IEnumerable<SoldProductDto> SoldProducts { get; set; }
    }
}
