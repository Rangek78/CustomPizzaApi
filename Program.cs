using CustomPizzaApi.Data;
using CustomPizzaApi.MappingProfiles;
using Mapster;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<PizzaContext>(options =>
        options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDefault"]),
        ServiceLifetime.Scoped);

builder.Services.AddMapster();
builder.Services.RegisterMappings();

// Lets enum value be converted to a string during JSON serialization
builder.Services.AddControllers()
.AddJsonOptions(opt=> { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
