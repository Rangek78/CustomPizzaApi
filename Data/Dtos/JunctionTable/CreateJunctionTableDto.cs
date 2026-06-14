using System.ComponentModel.DataAnnotations;
namespace CustomPizzaApi.Data.Dtos.JunctionTable;

public class CreateJunctionTableDto
{
    [Required]
    public int PizzaId { get; set; }
    [Required]
    public int IngredientId { get; set; }
    public uint IngredientAmount { get; set; } = 1;
}
