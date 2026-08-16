using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CustomPizzaApi.Tests;

public class PizzaWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            string? connectionString = context.Configuration["ConnectionStrings:PizzaTestsConnection"];
            if (string.IsNullOrEmpty(connectionString)) throw new InvalidOperationException("Test database connection string not found in configuration.");

            services.RemoveAll<DbContextOptions<PizzaContext>>();

            services.AddDbContext<PizzaContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        });
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        using var scope = Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<PizzaContext>();
        Console.WriteLine(context.Database.ProviderName);

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Console.WriteLine("Disposed");
    }
}
