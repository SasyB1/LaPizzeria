using LaPizzeria.Models;
using Newtonsoft.Json;
using LaPizzeria.Models.DTO;
using Microsoft.EntityFrameworkCore;
using LaPizzeria.Services.Interfaces;
namespace LaPizzeria.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _http;
        private readonly InFornoDbContext _context;

        public CartService(IHttpContextAccessor httpContextAccessor, InFornoDbContext context)
        {
            _http = httpContextAccessor;
            _context = context;
        }

        public List<OrderItem> GetCart()
        {
            var cartJson = _http.HttpContext.Session.GetString("Cart");
            return cartJson != null ? JsonConvert.DeserializeObject<List<OrderItem>>(cartJson) : new List<OrderItem>();
        }

        public void SaveCart(List<OrderItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            _http.HttpContext.Session.SetString("Cart", cartJson);
        }

        public void ClearCart()
        {
            _http.HttpContext.Session.Remove("Cart");
        }

        public async Task<int> SubmitOrderAsync(OrderDTO model, int userId, List<OrderItem> products)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    Address = model.Address,
                    User = await _context.Users.FindAsync(userId),
                    Note = model.Note,
                    DateTime = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in model.OrderItems)
                {
                    var product = products.FirstOrDefault(p => p.Product.ProductId == item.ProductId);
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {item.ProductId} not found.");
                    }

                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.OrderId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while submitting the order", ex);
            }
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new Exception($"Order with ID {orderId} not found.");
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving the order", ex);
            }
        }


        public void AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            var cart = GetCart();
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                var cartItem = cart.FirstOrDefault(item => item.Product.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new OrderItem
                    {
                        Product = product,
                        Quantity = quantity
                    });
                }
                SaveCart(cart);
            }
            else
            {
                throw new Exception($"Product with ID {productId} not found.");
            }
        }


        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.Product.ProductId == productId);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
        }

    }
}
