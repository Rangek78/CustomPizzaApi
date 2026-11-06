using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Tests.PizzaService;

[Collection("NoParallelization")]
public sealed partial class PizzaServiceIntegrationTests : IClassFixture<PizzaWebApplicationFactory>, IAsyncLifetime
{
    private readonly PizzaWebApplicationFactory app;

    public PizzaServiceIntegrationTests(PizzaWebApplicationFactory app) {
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

    private static Pizza GeneratePizza()
    {
        return new Pizza
        {
            Name = "Margherita",
            Size = PizzaSize.Small,
        };
    }
}
