using LaPizzeria.Models.DTO;
using LaPizzeria.Models;


namespace LaPizzeria.Services.Interfaces
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetAllIngredientsAsync();
        Task<Ingredient> GetIngredientByIdAsync(int id);
        Task AddIngredientAsync(IngredientDTO ingredientDto);
        Task UpdateIngredientAsync(int id, Ingredient ingredient);
        Task DeleteIngredientAsync(int id);
    }
}
