using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data;
using MapsterMapper;

namespace CustomPizzaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly PizzaContext _context;
    private readonly IMapper _mapper;

    public IngredientsController(PizzaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Data.Dtos.Ingredient.ReadIngredientDto>>> GetIngredients()
    {
        var ingredients = await _context.Ingredients.Include(i => i.Pizzas).ToListAsync();
        return Ok(_mapper.Map<List<Data.Dtos.Ingredient.ReadIngredientDto>>(ingredients));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIngredient(int id)
    {
        var ingredient = await FindIngredientAsync(id);
        if (ingredient == null) return NotFound();

        return Ok(_mapper.Map<Data.Dtos.Ingredient.ReadIngredientDto>(ingredient));
    }

    [HttpPost]
    public async Task<ActionResult<Ingredient>> PostIngredient(Ingredient ingredientDto)
    {
        _context.Ingredients.Add(ingredientDto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIngredient), new { id = ingredientDto.Id }, ingredientDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutIngredient(int id, Data.Dtos.Ingredient.UpdateIngredientDto ingredientDto)
    {
        var ingredient = await FindIngredientAsync(id);
        if (ingredient == null) return NotFound();

        _mapper.Map(ingredientDto, ingredient);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var ingredient = await FindIngredientAsync(id);
        if (ingredient == null) return NotFound();

        _context.Ingredients.Remove(ingredient);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Ingredient?> FindIngredientAsync(int id)
    {
        return await _context.Ingredients.Include(i => i.Pizzas).FirstOrDefaultAsync(p => p.Id == id);
    }

}

