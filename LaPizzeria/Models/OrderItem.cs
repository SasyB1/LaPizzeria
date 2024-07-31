using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LaPizzeria.Models
{
    public class OrderItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; } 

        [ForeignKey("Order")]
        public int OrderId { get; set; } 

        [ForeignKey("Product")]
        public int ProductId { get; set; } 

        [Required]
        public Product Product { get; set; } 

        [Required]
        public int Quantity { get; set; }
    }
}
