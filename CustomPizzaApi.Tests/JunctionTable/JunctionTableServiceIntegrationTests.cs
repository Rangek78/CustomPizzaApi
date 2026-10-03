using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Tests.JunctionTable;

[Collection("NoParallelization")]
public sealed partial class JunctionTableServiceIntegrationTests : IClassFixture<PizzaWebApplicationFactory>, IAsyncLifetime
{
    private readonly PizzaWebApplicationFactory app;

    public JunctionTableServiceIntegrationTests(PizzaWebApplicationFactory app) {
        this.app = app;
    }

    public async Task InitializeAsync()
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private (Pizza,Ingredient) GeneratePizzaAndIngredient()
    {
        var pizza = new Pizza
        {
            Name = "Margherita",
            Size = PizzaSize.Small,
        };

        var ingredient = new Ingredient()
        {
            Name = "Mozzarella cheese",
            Price = 10m,
        };

        return (pizza,ingredient);
    }

    private Models.JunctionTable GenerateJunctionTable(int pizzaId, int ingredientId)
    {
        return new Models.JunctionTable()
        {
            PizzaId = pizzaId,
            IngredientId = ingredientId,
            IngredientAmount = 1,
        };
    }
}
