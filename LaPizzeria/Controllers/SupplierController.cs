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
        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Ingredient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind("ProductName,ProductImage,Description,ProductPrice,ProductDeliveryTime,Ingredients")] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            _context.Products.Add(new Product
            {
                ProductName = product.ProductName,
                ProductImage = product.ProductImage,
                Description = product.Description,
                ProductPrice = product.ProductPrice,
                ProductDeliveryTime = product.ProductDeliveryTime,
                Ingredients = product.Ingredients
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct([Bind("ProductName,ProductImage,Description,ProductPrice,ProductDeliveryTime,Ingredients")] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            var productToUpdate = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == product.ProductName);
            if (productToUpdate == null)
            {
                return NotFound();
            }
            productToUpdate.ProductImage = product.ProductImage;
            productToUpdate.Description = product.Description;
            productToUpdate.ProductPrice = product.ProductPrice;
            productToUpdate.ProductDeliveryTime = product.ProductDeliveryTime;
            productToUpdate.Ingredients = product.Ingredients;
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct([Bind("ProductName")] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            var productToDelete = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == product.ProductName);
            if (productToDelete == null)
            {
                return NotFound();
            }
            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIngredient([Bind("IngredientName")] IngredientDTO ingredient)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredient);
            }
            _context.Ingredients.Add(new Ingredient
            {
                IngredientName = ingredient.IngredientName

            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredient");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateIngredient([Bind("IngredientName,Products")] IngredientDTO ingredient)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredient);
            }
            var ingredients = new Ingredient
            {
                IngredientName = ingredient.IngredientName

            };
            _context.Ingredients.Update(ingredients);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredient");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIngredient([Bind("IngredientId")] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
            {
                return View(ingredient);
            }
            var ingredientToDelete = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngredientName == ingredient.IngredientName);
            if (ingredientToDelete == null)
            {
                return NotFound();
            }
            _context.Ingredients.Remove(ingredientToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ingredient");
        }

    }
}
