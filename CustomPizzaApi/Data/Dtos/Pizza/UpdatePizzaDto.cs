using System.ComponentModel.DataAnnotations;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Data.Dtos.Pizza;

public class UpdatePizzaDto
{
    [Required]
    public required string Name { get; set; }
    [Required]
    public required PizzaSize? Size { get; set; }
}
