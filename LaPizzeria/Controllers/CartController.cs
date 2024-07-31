using LaPizzeria.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LaPizzeria.Models.DTO;
using LaPizzeria.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LaPizzeria.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;

        public CartController(ICartService cartService, IAuthService authService)
        {
            _cartService = cartService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            var cartViewModel = new CartDTO
            {
                Items = cart,
                TotalPrice = cart.Sum(item => item.Product.ProductPrice * item.Quantity)
            };
            return View(cartViewModel);
        }

        public async Task<IActionResult> Checkout()
        {
            var cart = _cartService.GetCart();
            var user = await _authService.GetCurrentUser();

            var orderDTO = new OrderDTO
            {
                OrderItems = cart.Select(c => new OrderItemDTO
                {
                    ProductId = c.Product.ProductId,
                    ProductName = c.Product.ProductName,
                    ProductPrice = c.Product.ProductPrice,
                    Quantity = c.Quantity
                }).ToList(),
                User = user,
                Address = "", 
                Note = "" 
            };

            return View(orderDTO);
        }

       

        [HttpPost]
        public async Task<IActionResult> SubmitOrder(OrderDTO model)
        {
            var userId = _authService.GetCurrentUserId();
            var cart = _cartService.GetCart();

            if (model.OrderItems == null || !model.OrderItems.Any())
            {
                ModelState.AddModelError("", "Il carrello è vuoto.");
                return View("Checkout", model);
            }

            try
            {
                var orderId = await _cartService.SubmitOrderAsync(model, userId, cart);
                _cartService.ClearCart();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Errore durante l'invio dell'ordine: {ex.Message}");
                return View("Checkout", model);
            }
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than zero.");
                return RedirectToAction("Index", "Cart"); 
            }

            try
            {
                _cartService.AddToCart(productId, quantity);
                return RedirectToAction("Index"); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding item to cart: {ex.Message}");
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart(); 
            return RedirectToAction("Index"); 
        }
    }
}
