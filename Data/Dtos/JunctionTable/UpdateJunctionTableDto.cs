using System.ComponentModel.DataAnnotations;
namespace CustomPizzaApi.Data.Dtos.JunctionTable;

public class UpdateJunctionTableDto
{
    [Required]
    public int PizzaId { get; set; }
    [Required]
    public int IngredientId { get; set; }
}
