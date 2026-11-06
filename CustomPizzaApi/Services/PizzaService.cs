using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data;
using MapsterMapper;
using FluentResults;

namespace CustomPizzaApi.Services;

public class PizzaService
{
    private readonly PizzaContext _context;
    private readonly IMapper _mapper;

    public PizzaService(PizzaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Data.Dtos.Pizza.ReadPizzaDto>> GetPizzas()
    {
        var pizzas = await _context.Pizzas.Include(p => p.Ingredients).ThenInclude(i => i.Ingredient).ToListAsync();
        return _mapper.Map<List<Data.Dtos.Pizza.ReadPizzaDto>>(pizzas);
    }

    public async Task<Result<Data.Dtos.Pizza.ReadPizzaDto>> GetPizza(int id)
    {
        var pizza = await FindPizzaAsync(id);
        if (pizza == null) return Result.Fail(FailCause.NotFound.ToString());

        return Result.Ok(_mapper.Map<Data.Dtos.Pizza.ReadPizzaDto>(pizza));
    }

    public async Task<Result<Pizza>> PostPizza(Data.Dtos.Pizza.CreatePizzaDto pizzaDto)
    {
        var pizza = _mapper.Map<Pizza>(pizzaDto);
        try
        {
            _context.Pizzas.Add(pizza);
            await _context.SaveChangesAsync();
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(FailCause.BadRequest.ToString());
        }
        catch (DbUpdateException)           // catches unique constraints, truncation, check constraints, etc.
        {
            return Result.Fail(FailCause.BadRequest.ToString());
        }
        return Result.Ok(pizza);
    }

    public async Task<Result> PutPizza(int id, Data.Dtos.Pizza.UpdatePizzaDto pizzaDto)
    {
        var pizza = await FindPizzaAsync(id);
        if (pizza == null) return Result.Fail(FailCause.NotFound.ToString());

        try
        {
            _mapper.Map(pizzaDto, pizza);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Result.Fail(FailCause.BadRequest.ToString());
        }
        catch (NullReferenceException)
        {
            return Result.Fail(FailCause.BadRequest.ToString());
        }
        return Result.Ok();
    }

    public async Task<Result> DeletePizza(int id)
    {
        var pizza = await FindPizzaAsync(id);
        if (pizza == null) return Result.Fail(FailCause.NotFound.ToString());

        _context.Pizzas.Remove(pizza);

        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<Pizza?> FindPizzaAsync(int id)
    {
        return await _context.Pizzas.Include(p => p.Ingredients).ThenInclude(i => i.Ingredient).FirstOrDefaultAsync(p => p.Id == id);
    }

}

