namespace CustomPizzaApi.Data.Dtos.JunctionTable;

public class ReadJunctionTableDto
{
    public int? PizzaId { get; set; }
    public int? IngredientId { get; set; }
    public uint IngredientAmount { get; set; }
}
