using LaPizzeria.Models;
using Microsoft.AspNetCore.Mvc;
using LaPizzeria.Models.DTO;
using LaPizzeria.Services.Interfaces;

namespace LaPizzeria.Controllers
{
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

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _productService.GetAllOrderAsync();
            var totalRevenue = await _productService.GetTotalIncomeAsync(DateTime.Today);

            ViewBag.TotalIncome = totalRevenue;

            return View(orders);
        }

        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductDTO productDto, IFormFile productImage, List<int> SelectedIngredients)
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
        public async Task<IActionResult> UpdateProduct(int id, Product product, IFormFile productImage, List<int> SelectedIngredients)
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
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> UpdateIngredient(int id, Ingredient model)
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
        [ValidateAntiForgeryToken]
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
                return Json(new { success = true });
            }
            catch (KeyNotFoundException)
            {
                return Json(new { success = false, message = "Ordine non trovato." });
            }
        }
    }
}
