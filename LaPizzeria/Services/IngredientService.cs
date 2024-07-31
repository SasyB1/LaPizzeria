using LaPizzeria.Models.DTO;
using LaPizzeria.Models;
using Microsoft.EntityFrameworkCore;
using LaPizzeria.Services.Interfaces;

namespace LaPizzeria.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly InFornoDbContext _context;

        public IngredientService(InFornoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(int id)
        {
            return await _context.Ingredients.FindAsync(id);
        }

        public async Task AddIngredientAsync(IngredientDTO ingredientDto)
        {
            var ingredient = new Ingredient
            {
                IngredientName = ingredientDto.IngredientName
            };

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateIngredientAsync(int id, Ingredient ingredient)
        {
            var ingredientToUpdate = await _context.Ingredients.FindAsync(id);
            if (ingredientToUpdate == null)
            {
                throw new KeyNotFoundException();
            }

            ingredientToUpdate.IngredientName = ingredient.IngredientName;
            _context.Ingredients.Update(ingredientToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteIngredientAsync(int id)
        {
            var ingredientToDelete = await _context.Ingredients.FindAsync(id);
            if (ingredientToDelete == null)
            {
                throw new KeyNotFoundException();
            }

            _context.Ingredients.Remove(ingredientToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
