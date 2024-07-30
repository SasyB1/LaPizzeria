using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LaPizzeria.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public Product Product { get; set; }  
        [Required]
       public int Quantity { get; set; }

    }
}
