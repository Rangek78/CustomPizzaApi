using CustomPizzaApi.Models;

namespace CustomPizzaApi.Data.Dtos;

public class ReadPizzaDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public PizzaSize Size { get; set; }
    public int SizeId { get; set; }
    public ICollection<IngredientInPizza>? Ingredients { get; set; }
}
