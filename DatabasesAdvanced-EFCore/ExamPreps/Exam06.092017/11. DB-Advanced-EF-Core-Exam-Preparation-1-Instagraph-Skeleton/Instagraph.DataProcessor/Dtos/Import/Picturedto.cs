namespace Instagraph.DataProcessor.Dtos.Import
{
    using System.ComponentModel.DataAnnotations;

    public class PictureDto
    {
        [Required]
        [MinLength(1)]
        public string Path { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Size { get; set; }
    }
}
