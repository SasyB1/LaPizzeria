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

        public CartController(ICartService cartService,IAuthService authService)
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
                User = user  
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
                ModelState.AddModelError("", "The cart is empty.");
                return View("Checkout", model);
            }

            try
            {
                var orderId = await _cartService.SubmitOrderAsync(model, userId, cart);
                _cartService.ClearCart();

                return RedirectToAction("OrderConfirmation", new { id = orderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error submitting order: {ex.Message}");
                return View("Checkout", model);
            }
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            try
            {
                var order = await _cartService.GetOrderAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return View(order);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving order: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            try
            {
                _cartService.AddToCart(productId, quantity);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Gestisci l'eccezione come preferisci
                ModelState.AddModelError("", $"Error adding product to cart: {ex.Message}");
                return RedirectToAction("Index", "Catalog"); // Redirect alla pagina del catalogo se c'è un errore
            }
        }
    }
}
