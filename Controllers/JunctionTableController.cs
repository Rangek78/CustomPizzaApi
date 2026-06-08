using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data;
using MapsterMapper;

namespace CustomPizzaApi.Controllers;

[ApiController]
[Route($"api/[controller]")]
public class JunctionTableController : ControllerBase
{
    private readonly PizzaContext _context;
    private readonly IMapper _mapper;

    public JunctionTableController(PizzaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Data.Dtos.JunctionTable.ReadJunctionTableDto>>> GetIngredientsInPizzas()
    {
        var jTable = await _context.IngredientInPizza.ToListAsync();
        return Ok(_mapper.Map<List<Data.Dtos.JunctionTable.ReadJunctionTableDto>>(jTable));
    }

    [HttpGet("{pizzaId}/{ingredientId}")]
    public async Task<IActionResult> GetIngredientInPizza(int pizzaId, int ingredientId)
    {
        var jTable = await FindJunctionTableAsync(pizzaId, ingredientId);
        if (jTable == null) return NotFound();

        return Ok(_mapper.Map<Data.Dtos.JunctionTable.ReadJunctionTableDto>(jTable));
    }

    [HttpPost]
    public async Task<ActionResult<JunctionTable>> AddIngredientInPizza(Data.Dtos.JunctionTable.CreateJunctionTableDto jTableDto)
    {
        var jTable = _mapper.Map<JunctionTable>(jTableDto);
        try
        {
            _context.IngredientInPizza.Add(jTable);
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIngredientInPizza), new { PizzaId = jTable.PizzaId, IngredientId = jTable.IngredientId }, jTableDto);
    }

    [HttpPut("{pizzaId}/{ingredientId}")]
    public async Task<IActionResult> UpdateIngredientInPizza(int pizzaId, int ingredientId, Data.Dtos.JunctionTable.UpdateJunctionTableDto jTableDto)
    {
        var jTable = await FindJunctionTableAsync(pizzaId, ingredientId);
        if (jTable == null) return NotFound();

        _mapper.Map(jTableDto, jTable);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{pizzaId}/addIngredients")]
    public async Task<IActionResult> AddListOfIngredients(int pizzaId, [FromBody] IEnumerable<int> ingredientsIds)
    {
        var pizza = await _context.Pizzas.FirstOrDefaultAsync(p => p.Id == pizzaId);
        if (pizza == null) return NotFound();

        Ingredient? ingr;
        List<int> notFound = [];

        foreach (var ingredientId in ingredientsIds)
        {
            ingr = await FindIngredientAsync(ingredientId);
            if (ingr == null)
                notFound.Add(ingredientId);

            if (notFound.Count == 0)
                _context.IngredientInPizza.Add(new JunctionTable { IngredientId = ingredientId, PizzaId = pizzaId });
        }
        if (notFound.Count > 0)
            return NotFound(notFound);

        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return Conflict(ex.Entries); }  // Appropriate HTTP response

        return NoContent();
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

