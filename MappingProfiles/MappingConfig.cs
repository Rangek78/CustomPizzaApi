namespace CustomPizzaApi.MappingProfiles;

using CustomPizzaApi.Models;
using CustomPizzaApi.Data.Dtos;
using Mapster;

public static class MappingConfig
{
    public static void RegisterMappings(this IServiceCollection services)
    {
        var globalConfig = TypeAdapterConfig.GlobalSettings;
        var config = new TypeAdapterConfig();

        globalConfig.NewConfig<Pizza, ReadPizzaDto>()
            .Map(readDto => readDto.SizeId, pizza => (int) pizza.Size);
    }
}

