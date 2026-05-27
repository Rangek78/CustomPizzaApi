using CustomPizzaApi.Models;

namespace CustomPizzaApi.Data.Dtos;

public class ReadIngredientDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public ICollection<IngredientInPizza>? Pizzas { get; set; }
}
