using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltraHotel.Application.Features.Bookings.Commands;
using UltraHotel.Application.Features.Bookings.Dtos;
using UltraHotel.Application.Features.Bookings.Queries;

namespace UltraHotel.WebApi.Controllers.Reservations;

/// <summary>Reservas de habitaciones (solo Viajeros).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/bookings")]
[Authorize(Policy = "TravelerOnly")]
[Produces("application/json")]
public class BookingsController(ISender sender) : ControllerBase
{
    /// <summary>Crea una nueva reserva.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateBookingCommand command, CancellationToken ct)
    {
        BookingDto result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Obtiene el detalle de una reserva.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        BookingDto booking = await sender.Send(new GetBookingQuery(id), ct);
        return Ok(booking);
    }
}
