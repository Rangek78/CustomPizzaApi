using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data;
using MapsterMapper;
using FluentResults;

namespace CustomPizzaApi.Services;

public class JunctionTableService
{
    private readonly PizzaContext _context;
    private readonly IMapper _mapper;

    public JunctionTableService(PizzaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Data.Dtos.JunctionTable.ReadJunctionTableDto>> GetIngredientsInPizzas()
    {
        var jTable = await _context.IngredientInPizza.ToListAsync();
        return _mapper.Map<List<Data.Dtos.JunctionTable.ReadJunctionTableDto>>(jTable);
    }

    public async Task<Result<Data.Dtos.JunctionTable.ReadJunctionTableDto>> GetIngredientInPizza(int pizzaId, int ingredientId)
    {
        var jTable = await FindJunctionTableAsync(pizzaId, ingredientId);
        if (jTable == null)
            return Result.Fail(FailCause.NotFound.ToString());

        var jTableDto = _mapper.Map<Data.Dtos.JunctionTable.ReadJunctionTableDto>(jTable);
        return Result.Ok(jTableDto);
    }

    public async Task<Result<Data.Dtos.JunctionTable.CreateJunctionTableDto>> AddIngredientInPizza(Data.Dtos.JunctionTable.CreateJunctionTableDto jTableDto)
    {
        var jTable = _mapper.Map<JunctionTable>(jTableDto);
        try
        {
            _context.IngredientInPizza.Add(jTable);
            await _context.SaveChangesAsync();
        }
        catch (InvalidOperationException)
        {
            return Result.Fail(FailCause.BadRequest.ToString());
        }
        catch (DbUpdateException)
        {
            return Result.Fail(FailCause.BadRequest.ToString());
        }

        return Result.Ok(jTableDto);
    }

    public async Task<Result> UpdateIngredientAmount(int pizzaId, int ingredientId, Data.Dtos.JunctionTable.UpdateJunctionTableDto jTableDto)
    {
        var jTable = await FindJunctionTableAsync(pizzaId, ingredientId);
        if (jTable == null)
            return Result.Fail(FailCause.NotFound.ToString());

        _mapper.Map(jTableDto, jTable);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> AddListOfIngredients(int pizzaId, [FromBody] IEnumerable<int> ingredientsIds)
    {
        var pizza = await _context.Pizzas.FirstOrDefaultAsync(p => p.Id == pizzaId);
        if (pizza == null) return Result.Fail("Pizza not found");

        Ingredient? ingr;
        CustomError<int> notFound = new NotFound<int>();

        foreach (var ingredientId in ingredientsIds)
        {
            ingr = await FindIngredientAsync(ingredientId);
            if (ingr == null)
                notFound.Add(ingredientId);

            if (notFound.Count == 0)
                _context.IngredientInPizza.Add(new JunctionTable { IngredientId = ingredientId, PizzaId = pizzaId });
        }
        if (notFound.Count > 0)
            return Result.Fail(notFound);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            CustomError<int> conflicts = new Conflict<int>();
            List<int> c = ex.Entries.Select(e => (JunctionTable)e.Entity).Select(j => j.IngredientId).OfType<int>().ToList();
            conflicts.AddMultiple(c);
            return Result.Fail(conflicts);
        }

        return Result.Ok();
    }

    public async Task<Result> RemoveIngredientFromPizza(int pizzaId, int ingredientId)
    {
        var jTable = await FindJunctionTableAsync(pizzaId, ingredientId);
        if (jTable == null)
            return Result.Fail(FailCause.NotFound.ToString());

        _context.IngredientInPizza.Remove(jTable);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<JunctionTable?> FindJunctionTableAsync(int pizzaId, int ingredientId)
    {
        return await _context.IngredientInPizza.FirstOrDefaultAsync(j => j.PizzaId == pizzaId && j.IngredientId == ingredientId);
    }

    private async Task<Pizza?> FindPizzaAsync(int pizzaId)
    {
        return await _context.Pizzas.FirstOrDefaultAsync(p => p.Id == pizzaId);
    }

    private async Task<Ingredient?> FindIngredientAsync(int ingredientId)
    {
        return await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == ingredientId);
    }
}

