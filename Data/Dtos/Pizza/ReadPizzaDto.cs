using CustomPizzaApi.Models;

namespace CustomPizzaApi.Data.Dtos.Pizza;

public class ReadPizzaDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public PizzaSize Size { get; set; }
    public int SizeId { get; set; }
    public ICollection<ReadPizzaIngredientsDto>? Ingredients { get; set; }
}

