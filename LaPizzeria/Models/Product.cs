using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LaPizzeria.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        [Required, StringLength(128)]
        public byte[] ProductImage { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, 100)]
        [Precision(2)]
        public decimal ProductPrice { get; set; }

        [Required]
        [Range(0, 60)]
        public int ProductDeliveryTime { get; set; }

        [Required]
        public List<Ingredient> Ingredients { get; set; } = [];

    }
}
