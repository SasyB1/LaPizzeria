using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LaPizzeria.Models
{
    public class Checkout
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckoutId { get; set; }
        [Required]
        public List<Order> Orders { get; set; }
        [Required]
        public string Address { get; set; }

        public string Note { get; set; }
        
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
