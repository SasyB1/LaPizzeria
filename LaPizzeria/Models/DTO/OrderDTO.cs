namespace LaPizzeria.Models.DTO
{
    public class OrderDTO
    {
        public OrderDTO()
        {
            OrderItems = new List<OrderItemDTO>();
        }
         public int OrderId { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
        public string Address { get; set; }

        public required User User { get; set; }
        public string Note { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public bool isPaid { get; set; } = false;
        
    }
}