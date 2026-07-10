using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltraHotel.Application.Features.Search.Dtos;
using UltraHotel.Application.Features.Search.Queries;

namespace UltraHotel.WebApi.Controllers.Reservations;

/// <summary>Búsqueda de habitaciones disponibles (acceso público).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/rooms")]
[AllowAnonymous]
[Produces("application/json")]
public class SearchController(ISender sender) : ControllerBase
{
    /// <summary>Busca habitaciones disponibles por ciudad, fechas y número de huéspedes.</summary>
    /// <param name="city">Ciudad del hotel.</param>
    /// <param name="checkIn">Fecha de entrada (yyyy-MM-dd).</param>
    /// <param name="checkOut">Fecha de salida (yyyy-MM-dd).</param>
    /// <param name="guests">Número mínimo de huéspedes que debe soportar la habitación.</param>
    /// <param name="ct">Token de cancelación.</param>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<RoomSearchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] string city,
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        [FromQuery] int guests,
        CancellationToken ct)
    {
        SearchRoomsQuery query = new(city, checkIn, checkOut, guests);
        IReadOnlyList<RoomSearchResult> results = await sender.Send(query, ct);
        return Ok(results);
    }
}
