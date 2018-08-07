namespace Instagraph.DataProcessor.Dtos.Import
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class UserDto
    {
        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(20)]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [JsonProperty(propertyName: "ProfilePicture")]
        public string ProfilePicturePath { get; set; }
    }
}
