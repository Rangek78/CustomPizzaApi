using System.Net.Http.Json;
using CustomPizzaApi.Data;
using CustomPizzaApi.Data.Dtos.Pizza;
using Microsoft.Extensions.DependencyInjection;
using CustomPizzaApi.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace CustomPizzaApi.Tests;

public sealed class PizzaControllerIntegrationTests : IClassFixture<PizzaWebApplicationFactory>, IAsyncLifetime
{
    private readonly PizzaWebApplicationFactory app;

    public PizzaControllerIntegrationTests(PizzaWebApplicationFactory app) {
        this.app = app;
    }

    [Fact]
    public async Task GetAllPizzas_Returns200()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var pizza = new Pizza
        {
            Name = "Margherita",
            Size = PizzaSize.Medium,
            Ingredients = []
        };
        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        var client = app.CreateClient();

        // Act
        var result = await client.GetAsync("api/pizzas/");
        var pizzaList = await result.Content.ReadFromJsonAsync<IEnumerable<ReadPizzaDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(pizzaList);

        var returnedPizza = Assert.Single(pizzaList);
        Assert.Equal(pizza.Id, returnedPizza.Id);
        Assert.Equal(pizza.Name, returnedPizza.Name);
        Assert.Equal(pizza.Size, returnedPizza.Size);
    }

    [Fact]
    public async Task GetById_Returns200_WhenPizzaExists()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var pizza = new Pizza
        {
            Name = "Margherita",
            Size = PizzaSize.Small,
        };
        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync($"api/pizzas/{pizza.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ReadPizzaDto>();

        Assert.NotNull(result);
        Assert.Equal(pizza.Id, result.Id);
        Assert.Equal(pizza.Name, result.Name);
        Assert.Equal(pizza.Size, result.Size);
    }

    [Fact]
    public async Task GetById_Returns404_WhenPizzaDoesNotExist()
    {
        // Arrange
        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync($"api/pizzas/0");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetByIdWithIngredients_JoinsTables_WhenPizzaHasIngredients()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var pizza = new Pizza
        {
            Name = "Mozzarella",
            Size = PizzaSize.Large,
        };
        var ingredient = new Ingredient()
        {
            Name = "Cheese",
            Price = 10.0M
        };
        context.Pizzas.Add(pizza);
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();

        var jTable = new Models.JunctionTable()
        {
            PizzaId = pizza.Id,
            IngredientId = ingredient.Id,
            IngredientAmount = 1u
        };

        context.IngredientInPizza.Add(jTable);

        await context.SaveChangesAsync();

        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync($"api/pizzas/{pizza.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ReadPizzaDto>();

        Assert.NotNull(result);
        Assert.Equal(pizza.Id, result.Id);
        Assert.Equal(pizza.Name, result.Name);
        Assert.Equal(pizza.Size, result.Size);
        Assert.NotNull(result.Ingredients);

        foreach (var ing in result.Ingredients)
        {
            Assert.Equal(ingredient.Id, ing.Id);
            Assert.Equal(ingredient.Name, ing.Name);
            Assert.Equal(ingredient.Price, ing.Price);
            Assert.Equal(1u, ing.IngredientAmount);
        }
    }

    [Fact]
    public async Task PostPizza_Returns201_WhenPizzaIsValid()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var pizza = new Pizza
        {
            Name = "Pepperoni",
            Size = PizzaSize.Large,
        };

        var client = app.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync($"api/pizzas/", pizza);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdPizza = response.Content;

        var result = await response.Content.ReadFromJsonAsync<Pizza>();

        Assert.NotNull(result);

        var savedPizza = await context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == result.Id);

        Assert.NotNull(savedPizza);
        Assert.Equal(pizza.Name, savedPizza.Name);
        Assert.Equal(pizza.Size, savedPizza.Size);
        Assert.Equal(pizza.Ingredients, savedPizza.Ingredients);
    }

    [Fact]
    public async Task PutPizza_Returns204_WhenRequestIsSuccessful()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var pizza = new Pizza
        {
            Name = "Neapolitan",
            Size = PizzaSize.Medium,
        };

        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        var updatedName = "Pepperoni";

        var client = app.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"api/pizzas/{pizza.Id}", new UpdatePizzaDto { Name = updatedName, Size = pizza.Size });

        var updatedPizza = await context.Pizzas.AsNoTracking().FirstAsync(p => p.Id == pizza.Id);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(updatedName, updatedPizza.Name);
    }

    [Fact]
    public async Task PutPizza_Returns404_WhenPizzaDoesNotExist()
    {
        // Arrange
        var client = app.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"api/pizzas/0", new UpdatePizzaDto { Name = "PizzaName", Size = PizzaSize.Small });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePizza_Returns204_WhenRequestIsSuccessful()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var pizza = new Pizza
        {
            Name = "Neapolitan",
            Size = PizzaSize.Medium,
        };

        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        var client = app.CreateClient();

        // Act
        var response = await client.DeleteAsync($"api/pizzas/{pizza.Id}");

        var pizzaSearch = await context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == pizza.Id);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Null(pizzaSearch);
    }

    [Fact]
    public async Task DeletePizza_Returns404_WhenPizzaDoesNotExist()
    {
        // Arrange
        var client = app.CreateClient();

        // Act
        var response = await client.DeleteAsync($"api/pizzas/0");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

}
