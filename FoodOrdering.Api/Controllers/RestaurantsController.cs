using FoodOrdering.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController(CatalogService catalog) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken ct)
        => Ok(await catalog.GetRestaurantsAsync(search, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOne(Guid id, CancellationToken ct)
        => await catalog.GetRestaurantAsync(id, ct) is { } r ? Ok(r) : NotFound(new { error = "Restaurant not found." });
}
