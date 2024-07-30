namespace LaPizzeria.Models.DTO
{
    public class IngredientDTO
    {
        public string IngredientName { get; set; }

        public List<Product> Products { get; set; } = [];
    }
}
