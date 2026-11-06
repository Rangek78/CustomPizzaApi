using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CustomPizzaApi.Tests.PizzaService;

public sealed partial class PizzaServiceIntegrationTests
{
    [Fact]
    public async Task GetPizzas_IsEmpty_WhenNoPizzasExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        // Act
        var result = await service.GetPizzas();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPizzas_IsNonEmpty_WhenPizzasExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        var pizza = GeneratePizza();
        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPizzas();

        // Assert
        var pizzaInDatabase = Assert.Single(result);

        Assert.Equal(pizzaInDatabase.Id, pizza.Id);
        Assert.Equal(pizzaInDatabase.Name, pizza.Name);
        Assert.Equal(pizzaInDatabase.Size, pizza.Size);
        Assert.Equal(pizzaInDatabase.SizeId, (int)pizza.Size);

        var pizzaIngredients = pizzaInDatabase.Ingredients!.ToList();
        var pizzaInDatabaseIngredients = pizzaInDatabase.Ingredients!.ToList();

        for (int i = 0; i < pizzaIngredients.Count; i++)
        {
            Assert.Equal(pizzaIngredients[i].Id, pizzaInDatabaseIngredients[i].Id);
            Assert.Equal(pizzaIngredients[i].Name, pizzaInDatabaseIngredients[i].Name);
        }
    }
}
