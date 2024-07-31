using LaPizzeria.Models.DTO;
using LaPizzeria.Models;

namespace LaPizzeria.Services.Interfaces
{
    public interface ICartService
    {
        List<OrderItem> GetCart();
        void SaveCart(List<OrderItem> cart);
        void ClearCart();
        Task<int> SubmitOrderAsync(OrderDTO model, int userId, List<OrderItem> cart);
        Task<Order> GetOrderAsync(int orderId);
        void AddToCart(int productId, int quantity);

        void RemoveFromCart(int productId);
    }
}
