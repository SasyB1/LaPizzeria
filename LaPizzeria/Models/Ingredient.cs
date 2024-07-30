using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LaPizzeria.Models
{
    public class Ingredient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IngredientId { get; set; }
        [Required]
        public string IngredientName { get; set; }
        
        public List<Product> Products { get; set; } = [];
    }
}
