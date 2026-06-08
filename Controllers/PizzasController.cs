using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CustomPizzaApi.Models;
using CustomPizzaApi.Data;
using MapsterMapper;

namespace CustomPizzaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
    private readonly PizzaContext _context;
    private readonly IMapper _mapper;

    public PizzasController(PizzaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Data.Dtos.Pizza.ReadPizzaDto>>> GetPizzas()
    {
        var pizzas = await _context.Pizzas.Include(p => p.Ingredients).ThenInclude(i => i.Ingredient).ToListAsync();
        return Ok(_mapper.Map<List<Data.Dtos.Pizza.ReadPizzaDto>>(pizzas));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPizza(int id)
    {
        var pizza = await FindPizzaAsync(id);
        if (pizza == null) return NotFound();

        return Ok(_mapper.Map<Data.Dtos.Pizza.ReadPizzaDto>(pizza));
    }

    [HttpPost]
    public async Task<ActionResult<Pizza>> PostPizza(Pizza pizzaDto)
    {
        _context.Pizzas.Add(pizzaDto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPizza), new { id = pizzaDto.Id }, pizzaDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPizza(int id, Data.Dtos.Pizza.UpdatePizzaDto pizzaDto)
    {
        var pizza = await FindPizzaAsync(id);
        if (pizza == null) return NotFound();

        _mapper.Map(pizzaDto, pizza);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePizza(int id)
    {
        var pizza = await FindPizzaAsync(id);
        if (pizza == null) return NotFound();

        _context.Pizzas.Remove(pizza);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<Pizza?> FindPizzaAsync(int id)
    {
        return await _context.Pizzas.Include(p => p.Ingredients).ThenInclude(i => i.Ingredient).FirstOrDefaultAsync(p => p.Id == id);
    }

}

