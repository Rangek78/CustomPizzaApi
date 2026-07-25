namespace CustomPizzaApi.Data.Dtos.Ingredient;

public class ReadIngredientsForPizzaDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public uint IngredientAmount { get; set; }
}
