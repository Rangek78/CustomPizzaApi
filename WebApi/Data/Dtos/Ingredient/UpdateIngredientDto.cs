using System.ComponentModel.DataAnnotations;

namespace CustomPizzaApi.Data.Dtos.Ingredient;

public class UpdateIngredientDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public decimal Price { get; set; }
}
