namespace SoftJail.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Mail
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z0-9\s]*str\.$")]
        public string Address { get; set; }

        public int PrisonerId { get; set; }
        [Required]
        public Prisoner Prisoner { get; set; }
    }
}
