using System.ComponentModel.DataAnnotations;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Data.Dtos.Pizza;

public class CreatePizzaDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public PizzaSize Size { get; set; }
    public ICollection<Models.JunctionTable> Ingredients { get; set; } = [];
}

