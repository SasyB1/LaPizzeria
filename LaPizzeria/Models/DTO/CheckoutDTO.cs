namespace LaPizzeria.Models.DTO
{
    public class CheckoutDTO
    {
        public int OrderId { get; set; }
        public string Address { get; set; }
        public bool isPaid { get; set; }
    }
}
