namespace Stations.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(8)]
        [RegularExpression(@"^[A-Z]{2}\d+$")]
        public string SeatingPlace { get; set; }

        public int TripId { get; set; }
        [Required]
        public Trip Trip { get; set; }

        public int? CustomerCardId { get; set; }
        public CustomerCard CustomerCard { get; set; }
    }
}
