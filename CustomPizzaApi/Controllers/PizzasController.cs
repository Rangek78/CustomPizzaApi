using Microsoft.AspNetCore.Mvc;
using CustomPizzaApi.Models;
using CustomPizzaApi.Services;
using CustomPizzaApi.Data.Dtos.Pizza;

namespace CustomPizzaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
    private readonly PizzaService _pizzaService;

    public PizzasController(PizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadPizzaDto>>> GetPizzas()
    {
        var result = await _pizzaService.GetPizzas();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPizza(int id)
    {
        var result = await _pizzaService.GetPizza(id);
        if (result.IsFailed) return NotFound();

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<Pizza>> PostPizza(CreatePizzaDto pizzaDto)
    {
        var result = await _pizzaService.PostPizza(pizzaDto);
        var createdPizza = result.Value;
        return CreatedAtAction(nameof(GetPizza), new { id = createdPizza.Id }, createdPizza);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPizza(int id, UpdatePizzaDto pizzaDto)
    {
        var result = await _pizzaService.PutPizza(id, pizzaDto);
        if (result.IsFailed) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePizza(int id)
    {
        var result = await _pizzaService.DeletePizza(id);
        if (result.IsFailed) return NotFound();

        return NoContent();
    }
}

