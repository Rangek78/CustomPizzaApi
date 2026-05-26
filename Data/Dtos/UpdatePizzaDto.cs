using System.ComponentModel.DataAnnotations;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Data.Dtos;

public class UpdatePizzaDto
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public PizzaSize Size { get; set; }
}
