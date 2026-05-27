using System.ComponentModel.DataAnnotations;

namespace CustomPizzaApi.Data.Dtos;

public class UpdateIngredientDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public decimal Price { get; set; }
}
