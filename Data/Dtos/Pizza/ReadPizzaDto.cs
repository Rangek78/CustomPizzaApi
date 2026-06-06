using CustomPizzaApi.Models;
using CustomPizzaApi.Data.Dtos.Ingredient;

namespace CustomPizzaApi.Data.Dtos.Pizza;

public class ReadPizzaDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public PizzaSize Size { get; set; }
    public int SizeId { get; set; }
    public ICollection<ReadIngredientsForPizzaDto>? Ingredients { get; set; }
}

