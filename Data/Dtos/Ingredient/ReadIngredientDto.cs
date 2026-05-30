namespace CustomPizzaApi.Data.Dtos.Ingredient;

public class ReadIngredientDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public ICollection<ReadIngredientInPizzaDto>? Pizzas { get; set; }
}
