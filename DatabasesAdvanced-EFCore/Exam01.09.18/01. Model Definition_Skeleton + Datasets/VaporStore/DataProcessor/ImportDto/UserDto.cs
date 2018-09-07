namespace VaporStore.DataProcessor.ImportDto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class UserDto
    {
        [Required]
        [RegularExpression(@"^[A-Z][a-z]*\s[A-Z][a-z]*$")]
        public string FullName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Range(3, 103)]
        public int Age { get; set; }

        [Required]
        [MinLength(1)]
        public ICollection<CardDto> Cards { get; set; }
    }
}
