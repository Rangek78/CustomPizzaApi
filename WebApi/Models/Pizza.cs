using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CustomPizzaApi.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
    [Required]
    public PizzaSize Size { get; set; }
    public ICollection<JunctionTable> Ingredients { get; set; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PizzaSize {
    Small = 0,
    Medium = 1,
    Large = 2
}

