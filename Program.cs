using CustomPizzaApi.Data;
using CustomPizzaApi.MappingProfiles;
using Mapster;
using System.Text.Json.Serialization;
using CustomPizzaApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration["ConnectionStrings:PizzaConnection"];

builder.AddDbContextService();
builder.Services.AddMapster();
builder.Services.RegisterMappings();

builder.Services.AddScoped<JunctionTableService>();

// Lets enum value be converted to a string during JSON serialization
builder.Services.AddControllers()
.AddJsonOptions(opt=> {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // fixes unwanted behavior of a reference loop between
        // pizza to ingredients and ingredients to pizza
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
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
