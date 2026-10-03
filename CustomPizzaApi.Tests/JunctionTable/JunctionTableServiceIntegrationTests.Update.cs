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
    public async Task UpdateIngredientAmount_IsSuccess_WhenPizzaAndIngredientExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();

        var (pizza, ingredient) = GeneratePizzaAndIngredient();
        context.Pizzas.Add(pizza);
        context.Ingredients.Add(ingredient);

        await context.SaveChangesAsync();

        var jTable = GenerateJunctionTable(pizza.Id, ingredient.Id);

        context.IngredientInPizza.Add(jTable);
        await context.SaveChangesAsync();

        var updatedAmount = jTable.IngredientAmount++;

        var updatedJTableDto = new UpdateJunctionTableDto()
        {
            IngredientAmount = updatedAmount
        };

        // Act
        var result = await service.UpdateIngredientAmount(pizza.Id, ingredient.Id, updatedJTableDto);

        // Assert
        Assert.True(result.IsSuccess);

        var jTableInDatabase = await context.IngredientInPizza.AsNoTracking().FirstOrDefaultAsync(j => j.PizzaId == pizza.Id && j.IngredientId == ingredient.Id);
        Assert.NotNull(jTableInDatabase);

        Assert.Equal(updatedAmount, jTableInDatabase.IngredientAmount);
    }

    [Fact]
    public async Task UpdateIngredientAmount_IsFailed_WhenRelationshipDoesNotExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();

        var updatedAmount = 10u;

        var updatedJTableDto = new UpdateJunctionTableDto()
        {
            IngredientAmount = updatedAmount
        };

        // Act
        var result = await service.UpdateIngredientAmount(1, 1, updatedJTableDto);

        // Assert
        Assert.True(result.IsFailed);
        var errMessage = Assert.Single(result.Errors).Message;
        Assert.Equal(FailCause.NotFound.ToString(), errMessage);
    }
}
