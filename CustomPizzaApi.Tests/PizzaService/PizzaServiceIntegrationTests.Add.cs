using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Models;
using Mapster;
using CustomPizzaApi.Data.Dtos.Pizza;

namespace CustomPizzaApi.Tests.PizzaService;

public sealed partial class PizzaServiceIntegrationTests
{
    [Fact]
    public async Task AddPizza_IsSuccess_WhenParametersExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        var pizzaDto = GeneratePizza().Adapt<CreatePizzaDto>();

        // Act
        var result = await service.PostPizza(pizzaDto);

        // Assert
        Assert.True(result.IsSuccess);

        var pizza = result.Value;

        var pizzaInDatabase = await context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == pizza.Id);
        Assert.NotNull(pizzaInDatabase);

        Assert.Equal(pizza.Name, pizzaInDatabase.Name);
        var pizzaIngredients = pizza.Ingredients.ToList();
        var pizzaInDatabaseIngredients = pizzaInDatabase.Ingredients.ToList();

        for (int i = 0; i < pizzaIngredients.Count; i++)
        { Assert.Equal(pizzaIngredients[i], pizzaInDatabaseIngredients[i]); }
    }

    [Fact]
    public async Task AddPizza_IsFailure_WhenAnyParameterDoesNotExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        var pizzaDto = new CreatePizzaDto()
        {
            Name = null!,
        };

        // Act
        var result1 = await service.PostPizza(new CreatePizzaDto(){ Name = null!, Size = PizzaSize.Small });
        var result2 = await service.PostPizza(new CreatePizzaDto(){ Name = "PizzaName" });

        // Assert
        Assert.True(result1.IsFailed);
        Assert.True(result2.IsFailed);

        var error1 = Assert.Single(result1.Errors).Message;
        var error2 = Assert.Single(result2.Errors).Message;

        Assert.Equal(FailCause.BadRequest.ToString(), error1);
        Assert.Equal(FailCause.BadRequest.ToString(), error2);
    }
}
