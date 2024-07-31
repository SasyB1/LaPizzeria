using LaPizzeria.Models;
using Microsoft.AspNetCore.Mvc;
using LaPizzeria.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace LaPizzeria.Controllers
{
    public class SupplierController : Controller
    {
        private readonly InFornoDbContext _context;

        public SupplierController(InFornoDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AllProducts()
        {
            var products = await _context.Products.Include(p => p.Ingredients).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> AllIngredients()
        {
            var ingredients = await _context.Ingredients.ToListAsync();
            return View(ingredients);
        }

        public IActionResult AllOrders()
        {
            return View();
        }

        public IActionResult AddProduct()
        {
            ViewBag.Ingredients = _context.Ingredients.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductDTO productDto, IFormFile productImage, List<int> SelectedIngredients)
        {
            if (productImage == null || productImage.Length == 0)
            {
                ModelState.AddModelError("ProductImage", "ProductImage is required.");
                ViewBag.Ingredients = _context.Ingredients.ToList();
                return View(productDto);
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await productImage.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                ProductImage = imageBytes,
                Description = productDto.Description,
                ProductPrice = productDto.ProductPrice,
                ProductDeliveryTime = productDto.ProductDeliveryTime,
                Ingredients = new List<Ingredient>()
            };

            if (SelectedIngredients != null && SelectedIngredients.Any())
            {
                foreach (var ingredientId in SelectedIngredients)
                {
                    var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                    if (ingredient != null)
                    {
                        product.Ingredients.Add(ingredient);
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllProducts");
        }

        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Ingredients = _context.Ingredients.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id, Product product, IFormFile productImage, List<int> SelectedIngredients)
        {
            var productToUpdate = await _context.Products
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (productToUpdate == null)
            {
                return NotFound();
            }
            if (productImage != null && productImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productImage.CopyToAsync(memoryStream);
                    productToUpdate.ProductImage = memoryStream.ToArray();
                }
            }
            else if (productToUpdate.ProductImage == null)
            {
                ModelState.AddModelError("ProductImage", "ProductImage is required.");
                return View(productToUpdate);
            }
            productToUpdate.ProductName = product.ProductName;
            productToUpdate.Description = product.Description;
            productToUpdate.ProductPrice = product.ProductPrice;
            productToUpdate.ProductDeliveryTime = product.ProductDeliveryTime;
            productToUpdate.Ingredients.Clear();
            if (SelectedIngredients != null && SelectedIngredients.Any())
            {
                foreach (var ingredientId in SelectedIngredients)
                {
                    var ingredient = await _context.Ingredients.FindAsync(ingredientId);
                    if (ingredient != null)
                    {
                        productToUpdate.Ingredients.Add(ingredient);
                    }
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("AllProducts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productToDelete = await _context.Products
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (productToDelete == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
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

            var ingredient = new Ingredient
            {
                IngredientName = ingredientDto.IngredientName
            };

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllIngredients");
        }

        public async Task<IActionResult> UpdateIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            var ingredients = new Ingredient
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName
            };

            return View(ingredients);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIngredient(int id, Ingredient model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ingredientToUpdate = await _context.Ingredients.FindAsync(id);
            if (ingredientToUpdate == null)
            {
                return NotFound();
            }

            ingredientToUpdate.IngredientName = model.IngredientName;
            _context.Ingredients.Update(ingredientToUpdate);
            await _context.SaveChangesAsync();

            return RedirectToAction("AllIngredients");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredientToDelete = await _context.Ingredients.FindAsync(id);
            if (ingredientToDelete == null)
            {
                return NotFound();
            }

            _context.Ingredients.Remove(ingredientToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllIngredients");
        }

    }
}
