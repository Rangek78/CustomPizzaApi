using Microsoft.EntityFrameworkCore;
using CustomPizzaApi.Models;

namespace CustomPizzaApi.Data;

public class PizzaContext : DbContext
{
    public PizzaContext(DbContextOptions<PizzaContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<JunctionTable>()
            .HasKey(ingInP => new { ingInP.PizzaId, ingInP.IngredientId });

        builder.Entity<JunctionTable>()
            .HasOne(ingInP => ingInP.Pizza)
            .WithMany(pizza => pizza.Ingredients)
            .HasForeignKey(ingInP => ingInP.PizzaId);

        builder.Entity<JunctionTable>()
            .HasOne(ingInP => ingInP.Ingredient)
            .WithMany(ingredient => ingredient.Pizzas)
            .HasForeignKey(ingInP => ingInP.IngredientId);
    }

    public DbSet<Pizza> Pizzas { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<JunctionTable> IngredientInPizza { get; set; } = null!;
}

