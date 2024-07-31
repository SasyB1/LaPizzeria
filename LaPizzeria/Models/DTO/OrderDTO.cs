namespace LaPizzeria.Models.DTO
{
    public class OrderDTO
    {
        public List<OrderItemDTO> OrderItems { get; set; }
        public string Address { get; set; }

        public required User User { get; set; }
        public string Note { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        
    }
}