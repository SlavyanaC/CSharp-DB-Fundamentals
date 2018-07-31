namespace ProductsShop.App.Dtos.Json.Export
{
    using Newtonsoft.Json;

    public class UsersColletionDto
    {
        [JsonProperty("usersCount")]
        public int Count { get; set; }

        [JsonProperty("users")]
        public UserDto[] Users { get; set; }
    }
}
