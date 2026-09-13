using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Services;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Tests.JunctionTable;

public sealed partial class JunctionTableServiceIntegrationTests
{
    [Fact]
    public async Task AddListOfIngredientInPizza_IsSuccess_WhenRelationshipsAreNew()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<JunctionTableService>();

        var (pizza, ingredient1) = GeneratePizzaAndIngredient();
        var ingredient2 = new Ingredient()
        {
            Name = "Tomato Sauce",
            Price = 3.4m
        };

        context.Pizzas.Add(pizza);
        context.Ingredients.Add(ingredient1);
        context.Ingredients.Add(ingredient2);

        await context.SaveChangesAsync();

        var listOfIngredientsIds = new List<int>() {ingredient1.Id, ingredient2.Id};

        // Act
        var result = await service.AddListOfIngredients(pizza.Id, listOfIngredientsIds);

        // Assert
        Assert.True(result.IsSuccess);

        var jTable1InDatabase = await context.IngredientInPizza.AsNoTracking().FirstOrDefaultAsync(j => j.PizzaId == pizza.Id && j.IngredientId == ingredient1.Id);
        var jTable2InDatabase = await context.IngredientInPizza.AsNoTracking().FirstOrDefaultAsync(j => j.PizzaId == pizza.Id && j.IngredientId == ingredient2.Id);
        Assert.NotNull(jTable1InDatabase);
        Assert.NotNull(jTable2InDatabase);

        Assert.Equal(ingredient1.Id, jTable1InDatabase.IngredientId);
        Assert.Equal(ingredient2.Id, jTable2InDatabase.IngredientId);
    }
}
