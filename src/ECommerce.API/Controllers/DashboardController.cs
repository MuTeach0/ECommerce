using Asp.Versioning;
using ECommerce.Application.Features.Dashboard.DTOs;
using ECommerce.Application.Features.Dashboard.Queries.GetOrderStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiVersion("2.0")]
[Authorize(Roles = "Admin")]
[Route("api/v{version:apiVersion}/dashboard")]
public class DashboardController(ISender sender) : ApiController
{
    //public async Task<IActionResult> GetTodayStats(
    //[FromQuery, ModelBinder(BinderType = typeof(DateOnlyModelBinder))] DateOnly? date,
    //CancellationToken ct)
    
    [HttpGet("stats")]
    [ProducesResponseType(typeof(TodayOrderStatsDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTodayStats([FromQuery] DateOnly? date, CancellationToken ct)
    {
        var statsDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await sender.Send(new GetOrderStatsQuery(statsDate), ct);

        return result.Match(
            response => Ok(response),
            Problem);
    }
}