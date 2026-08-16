using System.Net.Http.Json;
using CustomPizzaApi.Data;
using CustomPizzaApi.Data.Dtos.Pizza;
using Microsoft.Extensions.DependencyInjection;
using CustomPizzaApi.Models;
using System.Net;

namespace CustomPizzaApi.Tests;

public class PizzaControllerIntegrationTests : IClassFixture<PizzaWebApplicationFactory>
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
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(pizzaList);
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
            Ingredients = []
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
        Assert.Equivalent(pizza.Ingredients, result.Ingredients);
    }

    [Fact]
    public async Task GetById_Returns400_WhenPizzaDoesNotExist()
    {
        // Arrange
        var client = app.CreateClient();

        // Act
        var response = await client.GetAsync($"api/pizzas/0");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
