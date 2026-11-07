using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data.Dtos.Pizza;

namespace CustomPizzaApi.Tests.PizzaService;

public sealed partial class PizzaServiceIntegrationTests
{
    [Fact]
    public async Task UpdatePizza_IsSuccess_WhenPizzaExists()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        var pizza = GeneratePizza();

        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        var pizzaDto = new UpdatePizzaDto()
        {
            Name = "UpdatedPizza",
            Size = PizzaSize.Large
        };

        // Act
        var result = await service.PutPizza(pizza.Id, pizzaDto);

        // Assert
        Assert.True(result.IsSuccess);

        var pizzaInDatabase = await context.Pizzas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == pizza.Id);
        Assert.NotNull(pizzaInDatabase);

        Assert.Equal(pizzaDto.Name, pizzaInDatabase.Name);
    }

    [Fact]
    public async Task UpdatePizza_IsFailed_WhenPizzaDoesNotExist()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        var pizzaDto = new UpdatePizzaDto()
        {
            Name = "UpdatedPizza",
            Size = PizzaSize.Large
        };

        // Act
        var result = await service.PutPizza(1, pizzaDto);

        // Assert
        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors).Message;

        Assert.Equal(FailCause.NotFound.ToString(), error);
    }

    [Fact]
    public async Task UpdatePizza_IsFailed_WhenPizzaSizeIsNotProvided()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();
        var pizza = GeneratePizza();

        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        var pizzaDto = new UpdatePizzaDto()
        {
            Name = "Normal Pizza",
            Size = null
        };

        var newService = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();
        // Act
        var result = await newService.PutPizza(pizza.Id, pizzaDto);

        // Assert
        Assert.Null(pizzaDto.Size);
        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors).Message;

        Assert.Equal(FailCause.BadRequest.ToString(), error);
    }

    [Fact]
    public async Task UpdatePizza_IsFailed_WhenPizzaNameIsInvalid()
    {
        // Arrange
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        var service = scope.ServiceProvider.GetRequiredService<Services.PizzaService>();

        var pizza = GeneratePizza();

        context.Pizzas.Add(pizza);
        await context.SaveChangesAsync();

        var pizzaDto = new UpdatePizzaDto()
        {
            Name = null!,
            Size = PizzaSize.Medium
        };

        // Act
        var result = await service.PutPizza(pizza.Id, pizzaDto);

        // Assert
        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors).Message;

        Assert.Equal(FailCause.BadRequest.ToString(), error);
    }
}
