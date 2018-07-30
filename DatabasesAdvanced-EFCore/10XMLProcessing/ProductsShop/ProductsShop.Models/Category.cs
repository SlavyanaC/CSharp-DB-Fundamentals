namespace ProductsShop.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        public Category()
        {
            this.CategoryProducts = new HashSet<CategoryProducts>();
        }

        [Key]
        public int Id { get; set; }

        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        public ICollection<CategoryProducts> CategoryProducts { get; set; }
    }
}
