using Asp.Versioning;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UltraHotel.Application.Features.Rooms.Commands;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Commons.Dtos;

namespace UltraHotel.WebApi.Controllers.Hotels;

/// <summary>Gestión de habitaciones por hotel (solo Agentes).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hotels/{hotelId:guid}/rooms")]
[Authorize(Policy = "AgentOnly")]
[Produces("application/json")]
public class RoomsController(ISender sender) : ControllerBase
{
    /// <summary>Agrega una habitación al hotel.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid hotelId, [FromBody] CreateRoomRequest request, CancellationToken ct)
    {
        CreateRoomCommand command = request.Adapt<CreateRoomCommand>() with { HotelId = hotelId };
        RoomDto result = await sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Actualiza los datos de una habitación.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid hotelId, Guid id, [FromBody] UpdateRoomRequest request, CancellationToken ct)
    {
        UpdateRoomCommand command = request.Adapt<UpdateRoomCommand>() with { RoomId = id };
        RoomDto result = await sender.Send(command, ct);
        return Ok(result);
    }

    /// <summary>Habilita o deshabilita una habitación.</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleStatus(Guid hotelId, Guid id, [FromBody] ToggleStatusRequest request, CancellationToken ct)
    {
        await sender.Send(new ToggleRoomStatusCommand(id, request.IsEnabled), ct);
        return NoContent();
    }
}
