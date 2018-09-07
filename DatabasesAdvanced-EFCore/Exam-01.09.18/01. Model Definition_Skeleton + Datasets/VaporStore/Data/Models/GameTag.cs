namespace VaporStore.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class GameTag
    {
        [Required]
        public int GameId { get; set; }
        [Required]
        public Game Game { get; set; }

        [Required]
        public int TagId { get; set; }
        [Required]
        public Tag Tag { get; set; }
    }
}
