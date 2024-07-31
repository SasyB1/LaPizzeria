using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LaPizzeria.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Required]
        public List<OrderItem> OrderItems { get; set; }
        [Required]
        public string Address { get; set; }

        public required User User { get; set; }
        public string Note { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public bool isPaid { get; set; } = false;
    }
}
