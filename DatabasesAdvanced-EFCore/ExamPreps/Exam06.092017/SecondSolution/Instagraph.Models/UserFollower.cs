using System.ComponentModel.DataAnnotations;

namespace Instagraph.Models
{
    public class UserFollower
    {
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }

        public int FollowerId { get; set; }
        [Required]
        public User Follower { get; set; }
    }
}
