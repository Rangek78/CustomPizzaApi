using System.ComponentModel.DataAnnotations;
namespace CustomPizzaApi.Data.Dtos.JunctionTable;

public class UpdateJunctionTableDto
{
    [Required]
    public uint IngredientAmount { get; set; }
}
