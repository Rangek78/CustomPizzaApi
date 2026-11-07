using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data.Dtos.Pizza;

namespace CustomPizzaApi.Tests.PizzaService;

public sealed partial class PizzaServiceIntegrationTests
{
    [Fact]
    public async Task DeletePizza_IsSuccess_WhenPizzaExists()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        var pizza = GeneratePizza();

        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeletePizza(pizza.Id);

        // Assert
        Assert.True(result.IsSuccess);

        var pizzaInDatabase = await context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == pizza.Id);
        Assert.Null(pizzaInDatabase);
    }

    [Fact]
    public async Task DeletePizza_IsFailed_WhenPizzaDoesNotExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        // Act
        var result = await service.DeletePizza(0);

        // Assert
        Assert.True(result.IsFailed);
        var errorMessage = Assert.Single(result.Errors).Message;

        Assert.Equal(FailCause.NotFound.ToString(), errorMessage);
    }
}
