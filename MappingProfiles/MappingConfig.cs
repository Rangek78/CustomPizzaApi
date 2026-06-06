namespace CustomPizzaApi.MappingProfiles;

using CustomPizzaApi.Models;
using CustomPizzaApi.Data.Dtos.Pizza;
using CustomPizzaApi.Data.Dtos.Ingredient;
using Mapster;

public static class MappingConfig
{
    public static void RegisterMappings(this IServiceCollection services)
    {
        var globalConfig = TypeAdapterConfig.GlobalSettings;
        var config = new TypeAdapterConfig();

        globalConfig.NewConfig<Pizza, ReadPizzaDto>()
            .Map(readDto => readDto.SizeId, pizza => (int) pizza.Size)
            .Map(readDto => readDto.Ingredients, pizza => pizza.Ingredients.Select(i => i.Ingredient).ToList());

        globalConfig.NewConfig<Ingredient, ReadIngredientDto>()
            .Map(readDto => readDto.Pizzas, ingredient => ingredient.Pizzas.Select(i => i.Pizza).ToList());
    }
}

