using System.ComponentModel.DataAnnotations;

namespace CustomPizzaApi.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
    [Required]
    public PizzaSize Size { get; set; }
    public ICollection<IngredientInPizza> Ingredients { get; set; } = [];
}

public enum PizzaSize {
    Small = 0,
    Medium = 1,
    Large = 2
}

