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

        public IActionResult Ingredients()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductDTO productDto, IFormFile productImage)
        {
            if (productImage == null || productImage.Length == 0)
            {
                ModelState.AddModelError("ProductImage", "ProductImage is required.");
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
                Ingredients = productDto.Ingredients.Select(i => new Ingredient
                {
                    IngredientName = i.IngredientName
                }).ToList()
            };

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

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id, Product product, IFormFile productImage)
        {
            if (productImage != null && productImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productImage.CopyToAsync(memoryStream);
                    product.ProductImage = memoryStream.ToArray();
                }
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var productToUpdate = await _context.Products
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.ProductName = product.ProductName;
            productToUpdate.Description = product.Description;
            productToUpdate.ProductPrice = product.ProductPrice;
            productToUpdate.ProductDeliveryTime = product.ProductDeliveryTime;

            _context.Entry(productToUpdate).Collection(p => p.Ingredients).Load();
            productToUpdate.Ingredients.Clear();
            foreach (var ingredient in product.Ingredients)
            {
                productToUpdate.Ingredients.Add(new Ingredient
                {
                    IngredientName = ingredient.IngredientName
                });
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
            return RedirectToAction("Ingredients");
        }

        public IActionResult UpdateIngredient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIngredient([Bind("IngredientName")] IngredientDTO ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredientDto);
            }

            var ingredientToUpdate = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.IngredientName == ingredientDto.IngredientName);

            if (ingredientToUpdate == null)
            {
                return NotFound();
            }

            ingredientToUpdate.IngredientName = ingredientDto.IngredientName;
            _context.Ingredients.Update(ingredientToUpdate);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredients");
        }

        public IActionResult DeleteIngredient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIngredient([Bind("IngredientName")] IngredientDTO ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredientDto);
            }

            var ingredientToDelete = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.IngredientName == ingredientDto.IngredientName);

            if (ingredientToDelete == null)
            {
                return NotFound();
            }

            _context.Ingredients.Remove(ingredientToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredients");
        }
    }
}
