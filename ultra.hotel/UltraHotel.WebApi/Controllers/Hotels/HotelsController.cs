using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltraHotel.Application.Features.Hotels.Commands;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Application.Features.Hotels.Queries;
using UltraHotel.Commons.Dtos;

namespace UltraHotel.WebApi.Controllers.Hotels;

/// <summary>Gestión de hoteles (solo Agentes).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hotels")]
[Authorize(Policy = "AgentOnly")]
[Produces("application/json")]
public class HotelsController(ISender sender) : ControllerBase
{
    /// <summary>Crea un nuevo hotel.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(HotelDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHotelCommand command, CancellationToken ct)
    {
        HotelDto result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetBookings), new { id = result.Id }, result);
    }

    /// <summary>Actualiza los datos de un hotel.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHotelRequest request, CancellationToken ct)
    {
        UpdateHotelCommand command = request.Adapt<UpdateHotelCommand>() with { HotelId = id };
        HotelDto result = await sender.Send(command, ct);
        return Ok(result);
    }

    /// <summary>Habilita o deshabilita un hotel.</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request, CancellationToken ct)
    {
        await sender.Send(new ToggleHotelStatusCommand(id, request.IsEnabled), ct);
        return NoContent();
    }

    /// <summary>Obtiene las reservas de un hotel.</summary>
    [HttpGet("{id:guid}/bookings")]
    [ProducesResponseType(typeof(IReadOnlyList<HotelBookingSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookings(Guid id, CancellationToken ct)
    {
        IReadOnlyList<HotelBookingSummaryDto> bookings = await sender.Send(new GetHotelBookingsQuery(id), ct);
        return Ok(bookings);
    }
}
