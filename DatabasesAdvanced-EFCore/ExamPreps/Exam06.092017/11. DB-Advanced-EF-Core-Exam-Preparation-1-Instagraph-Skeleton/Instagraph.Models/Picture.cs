namespace Instagraph.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Picture
    {
        public Picture()
        {
            this.Users = new HashSet<User>();
            this.Posts = new HashSet<Post>();
        }

        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public decimal Size { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
