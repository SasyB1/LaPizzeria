namespace LaPizzeria.Models.DTO
{
    public class CartDTO
    {
        public List<OrderItem> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
