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
        builder.Entity<IngredientInPizza>()
            .HasKey(ingInP => new { ingInP.PizzaId, ingInP.IngredientId });

        builder.Entity<IngredientInPizza>()
            .HasOne(ingInP => ingInP.Pizza)
            .WithMany(pizza => pizza.Ingredients)
            .HasForeignKey(ingInP => ingInP.PizzaId);

        builder.Entity<IngredientInPizza>()
            .HasOne(ingInP => ingInP.Ingredient)
            .WithMany(ingredient => ingredient.Pizzas)
            .HasForeignKey(ingInP => ingInP.IngredientId);
    }

    public DbSet<Pizza> Pizzas { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<IngredientInPizza> IngredientInPizza { get; set; } = null!;
}

