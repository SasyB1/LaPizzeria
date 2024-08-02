using LaPizzeria.Models;
using Microsoft.AspNetCore.Mvc;
using LaPizzeria.Models.DTO;
using LaPizzeria.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LaPizzeria.Controllers
{
    [Authorize(Policy = Policies.IsAdmin)]
    public class SupplierController : Controller
    {
        private readonly IProductService _productService;
        private readonly IIngredientService _ingredientService;
        

        public SupplierController(IProductService productService, IIngredientService ingredientService)
        {
            _productService = productService;
            _ingredientService = ingredientService;
        }

        public async Task<IActionResult> AllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> AllIngredients()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            return View(ingredients);
        }

        public async Task<IActionResult> AllOrders(DateTime? date)
        {
            var currentDate = date ?? DateTime.Today;
            var orders = await _productService.GetAllOrderAsync();
            var filteredOrders = orders.Where(o => o.DateTime.Date == currentDate.Date).ToList();

            var totalPaidOrders = await _productService.GetTotalPaidOrdersAsync(currentDate);
            var totalIn = await _productService.GetTotalIncomeAsync(currentDate);

            ViewBag.TotalPaidOrders = totalPaidOrders;
            ViewBag.TotalIncome = totalIn.ToString("F2");
            ViewBag.SelectedDate = currentDate.ToString("yyyy-MM-dd");

            return View(filteredOrders);
        }


        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind("ProductName,ProductPrice,ProductDeliveryTime,Description")] ProductDTO productDto, IFormFile productImage, List<int> SelectedIngredients)
        {
            if (productImage == null || productImage.Length == 0)
            {
                ModelState.AddModelError("ProductImage", "ProductImage is required.");
                ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
                return View(productDto);
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await productImage.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            await _productService.AddProductAsync(productDto, imageBytes, SelectedIngredients);
            return RedirectToAction("AllProducts");
        }

        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id, [Bind("ProductName,ProductPrice,ProductDeliveryTime,Description")] Product product, IFormFile productImage, List<int> SelectedIngredients)
        {
            byte[] imageBytes = null;
            if (productImage != null && productImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productImage.CopyToAsync(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
            }

            try
            {
                await _productService.UpdateProductAsync(id, product, imageBytes, SelectedIngredients);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("ProductImage", ex.Message);
                return View(product);
            }

            return RedirectToAction("AllProducts");
        }

        [HttpPost]        
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction("AllProducts");
        }

        public IActionResult AddIngredient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIngredient([Bind("IngredientName")] IngredientDTO ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredientDto);
            }

            await _ingredientService.AddIngredientAsync(ingredientDto);
            return RedirectToAction("AllIngredients");
        }

        public async Task<IActionResult> UpdateIngredient(int id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIngredient([Bind("IngredientName")] int id, Ingredient model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _ingredientService.UpdateIngredientAsync(id, model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction("AllIngredients");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            try
            {
                await _ingredientService.DeleteIngredientAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction("AllIngredients");
        }

        [HttpPost]
        public async Task<IActionResult> MarkOrderIsPaid(int orderId)
        {
            try
            {
                await _productService.OrderIsPaidAsync(orderId);
                var totalPaidOrders = await _productService.GetTotalPaidOrdersAsync(DateTime.Now);
                var totalIncome = await _productService.GetTotalIncomeAsync(DateTime.Now);
                return Json(new
                {
                    success = true,
                    totalPaidOrders = totalPaidOrders,
                    totalIncome = totalIncome
                });
            }
            catch (KeyNotFoundException)
            {
                return Json(new { success = false, message = "Ordine non trovato." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Si è verificato un errore." });
            }
        }
    }
}
