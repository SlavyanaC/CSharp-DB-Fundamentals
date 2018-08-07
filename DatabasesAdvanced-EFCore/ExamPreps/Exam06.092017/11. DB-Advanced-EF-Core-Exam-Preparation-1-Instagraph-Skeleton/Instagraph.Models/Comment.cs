namespace Instagraph.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Content { get; set; }

        public int UserId { get; set; }
        [Required]
        public User User { get; set; }

        public int PostId { get; set; }
        [Required]
        public Post Post { get; set; }
    }
}
