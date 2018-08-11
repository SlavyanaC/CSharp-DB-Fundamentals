using System.ComponentModel.DataAnnotations;

namespace Instagraph.DataProcessor.Dto.Import
{
    public class UserFollowerDto
    {
        [Required]
        [MaxLength(30)]
        public string User { get; set; }

        [Required]
        [MaxLength(30)]
        public string Follower { get; set; }
    }
}
