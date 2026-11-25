using System.Security.Claims;
using FoodOrdering.Application.DTOs;
using FoodOrdering.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(OrderService orders) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest req, CancellationToken ct)
    {
        try { return Ok(await orders.CreateAsync(UserId, req, ct)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> Mine(CancellationToken ct) => Ok(await orders.GetMineAsync(UserId, ct));

    [HttpPost("{id:guid}/advance")]
    public async Task<IActionResult> Advance(Guid id, CancellationToken ct)
    {
        try { return Ok(await orders.AdvanceStatusAsync(UserId, id, ct)); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }
}
