using Microsoft.AspNetCore.Mvc;
using CustomPizzaApi.Models;
using CustomPizzaApi.Services;

namespace CustomPizzaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JunctionTableController : ControllerBase
{
    private readonly JunctionTableService _jTableService;

    public JunctionTableController(JunctionTableService jTableService)
    {
        _jTableService= jTableService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Data.Dtos.JunctionTable.ReadJunctionTableDto>>> GetIngredientsInPizzas()
    {
        var result = await _jTableService.GetIngredientsInPizzas();
        return Ok(result);
    }

    [HttpGet("{pizzaId}/{ingredientId}")]
    public async Task<IActionResult> GetIngredientInPizza(int pizzaId, int ingredientId)
    {
        var result = await _jTableService.GetIngredientInPizza(pizzaId, ingredientId);
        if (result.IsFailed) return NotFound();

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<JunctionTable>> AddIngredientInPizza(Data.Dtos.JunctionTable.CreateJunctionTableDto jTableDto)
    {
        var result = await _jTableService.AddIngredientInPizza(jTableDto);
        if (result.IsFailed) return BadRequest();
        var jTable = result.Value;

        return CreatedAtAction(nameof(GetIngredientInPizza), new { PizzaId = jTable.PizzaId, IngredientId = jTable.IngredientId , IngredientAmount = jTable.IngredientAmount }, jTableDto);
    }

    [HttpPut("{pizzaId}/{ingredientId}")]
    public async Task<IActionResult> UpdateIngredientAmount(int pizzaId, int ingredientId, Data.Dtos.JunctionTable.UpdateJunctionTableDto jTableDto)
    {
        var result = await _jTableService.UpdateIngredientAmount(pizzaId, ingredientId, jTableDto);
        if (result.IsFailed) return NotFound();

        return NoContent();
    }

    [HttpPut("{pizzaId}/addIngredients")]
    public async Task<IActionResult> AddListOfIngredients(int pizzaId, [FromBody] IEnumerable<int> ingredientsIds)
    {
        var result = await _jTableService.AddListOfIngredients(pizzaId, ingredientsIds);
        if (result.IsFailed)
        {
            if (result.HasError<NotFound<int>>(out var nfErrors))
            {
                var idsNotFound = new List<int>();
                foreach (var error in nfErrors)
                    idsNotFound.AddRange(error.Values);

                return NotFound(new { ingredientIdsNotFound = idsNotFound });
            }
            if (result.HasError<Conflict<int>>(out var cErrors))
            {
                var conflictingIds = new List<int>();
                foreach (var error in cErrors)
                    conflictingIds.AddRange(error.Values);

                return Conflict(new { conflictingIngredients = conflictingIds });
            }
            else            // has to be 'Pizza Id not found'
                return NotFound(new { pizzaId = pizzaId });
        }

        return NoContent();
    }

    [HttpDelete("{pizzaId}/{ingredientId}")]
    public async Task<IActionResult> RemoveIngredientFromPizza(int pizzaId, int ingredientId)
    {
        var result = await _jTableService.RemoveIngredientFromPizza(pizzaId, ingredientId);

        if (result.IsFailed) return NotFound();
        return NoContent();
    }
}

