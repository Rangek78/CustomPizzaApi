using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Data;

public static class DbExtension
{
    public static void AddDbContextService(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["ConnectionStrings:PizzaConnection"];
        builder.Services.AddDbContext<PizzaContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)),
                ServiceLifetime.Transient);
    }
}
