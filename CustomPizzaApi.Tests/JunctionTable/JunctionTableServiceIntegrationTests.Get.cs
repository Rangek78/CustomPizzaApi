using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using CustomPizzaApi.Services;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Tests.JunctionTable;

public sealed partial class JunctionTableServiceIntegrationTests
{
    [Fact]
    public async Task GetIngredientsInPizzas_ReturnsEmptyList_WhenNoIngredientsAreAssociatedToAnyPizzas()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();

        // Act
        var result = await service.GetIngredientsInPizzas();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetIngredientsInPizzas_ReturnsListOfIds_WhenPizzasHaveIngredients()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();

        var (pizza, ingredient) = GeneratePizzaAndIngredient();
        context.Pizzas.Add(pizza);
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();

        var jTable = GenerateJunctionTable(pizza.Id, ingredient.Id);

        context.IngredientInPizza.Add(jTable);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetIngredientsInPizzas();

        // Assert
        Assert.NotNull(result);
        var relationship = Assert.Single(result);

        Assert.Equal(jTable.PizzaId, relationship.PizzaId);
        Assert.Equal(jTable.IngredientId, relationship.IngredientId);
        Assert.Equal(jTable.IngredientAmount, relationship.IngredientAmount);
    }

    [Fact]
    public async Task GetIngredientsInPizzaById_ReturnsOkWithJunctionTable_WhenRelationshipExists()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();
        var (pizza, ingredient) = GeneratePizzaAndIngredient();
        context.Pizzas.Add(pizza);
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();

        var jTable = GenerateJunctionTable(pizza.Id, ingredient.Id);

        context.IngredientInPizza.Add(jTable);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetIngredientInPizza(pizza.Id, ingredient.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var relationship = result.Value;

        Assert.Equal(jTable.PizzaId, relationship.PizzaId);
        Assert.Equal(jTable.IngredientId, relationship.IngredientId);
        Assert.Equal(jTable.IngredientAmount, relationship.IngredientAmount);
    }

    [Fact]
    public async Task GetIngredientsInPizzaById_ReturnsNotFoundResult_WhenRelationshipDoesNotExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();
        var (pizza, ingredient) = GeneratePizzaAndIngredient();
        context.Pizzas.Add(pizza);
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetIngredientInPizza(pizza.Id, ingredient.Id);

        // Assert
        Assert.True(result.IsFailed);
        var errMessage = Assert.Single(result.Errors).Message;
        Assert.Equal(FailCause.NotFound.ToString(), errMessage);
    }
}
