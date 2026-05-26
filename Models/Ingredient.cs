using System.ComponentModel.DataAnnotations;

namespace CustomPizzaApi.Models;

public class Ingredient
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public required ICollection<IngredientInPizza> Pizzas { get; set; }
}

