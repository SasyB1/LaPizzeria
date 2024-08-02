using LaPizzeria.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LaPizzeria.Models.DTO;
using LaPizzeria.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LaPizzeria.Controllers
{
    [Authorize(Policy = Policies.SupplierOrCustomer)]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOrder([Bind("OrderId,OrderItems,Address,User,Note,DateTime,isPaid")] OrderDTO model)
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
                TempData["SuccessMessage"] = "Ordine inviato con successo!";
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
                ModelState.AddModelError("", "La quantita' dev essere maggiore di 0.");
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                _cartService.AddToCart(productId, quantity);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Errore durante l'aggiunta al carrello: {ex.Message}");
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            if (productId <= 0)
            {
                ModelState.AddModelError("", "Prodotto non valido.");
                return RedirectToAction("Index");
            }

            try
            {
                _cartService.RemoveFromCart(productId);
                TempData["SuccessMessage"] = "Prodotto rimosso con successo!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Errore durante la rimozione: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}
