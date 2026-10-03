using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Services;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data.Dtos.JunctionTable;

namespace CustomPizzaApi.Tests.JunctionTable;

public sealed partial class JunctionTableServiceIntegrationTests
{
    [Fact]
    public async Task AddIngredientInPizza_IsSuccessWithJunctionTable_WhenRelationshipIsNew()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();

        var (pizza, ingredient) = GeneratePizzaAndIngredient();
        context.Pizzas.Add(pizza);
        context.Ingredients.Add(ingredient);

        await context.SaveChangesAsync();

        var jTable = new CreateJunctionTableDto()
        {
            PizzaId = pizza.Id,
            IngredientId = ingredient.Id
        };

        // Act
        var result = await service.AddIngredientInPizza(jTable);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var jTableInDatabase = await context.IngredientInPizza.AsNoTracking().FirstOrDefaultAsync(j => j.PizzaId == pizza.Id && j.IngredientId == ingredient.Id);
        Assert.NotNull(jTableInDatabase);

        Assert.Equal(jTable.PizzaId, jTableInDatabase.PizzaId);
        Assert.Equal(jTable.IngredientId, jTableInDatabase.IngredientId);
        Assert.Equal(jTable.IngredientAmount, jTableInDatabase.IngredientAmount);
    }

    [Fact]
    public async Task AddIngredientInPizza_IsFailed_WhenRelationshipAlreadyExists()
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

        var existingJTable = new CreateJunctionTableDto()
        {
            PizzaId = pizza.Id,
            IngredientId = ingredient.Id
        };

        // Act
        var result = await service.AddIngredientInPizza(existingJTable);

        // Assert
        Assert.True(result.IsFailed);
        var errMessage = Assert.Single(result.Errors).Message;
        Assert.Equal(FailCause.BadRequest.ToString(), errMessage);
    }

    [Fact]
    public async Task AddIngredientInPizza_IsFailed_WhenPizzaOrIngredientDoesNotExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();

        var jTable = new CreateJunctionTableDto()
        {
            PizzaId = 0,
            IngredientId = 0
        };

        // Act
        var result = await service.AddIngredientInPizza(jTable);

        // Assert
        Assert.True(result.IsFailed);
        var errMessage = Assert.Single(result.Errors).Message;
        Assert.Equal(FailCause.BadRequest.ToString(), errMessage);
    }
}
