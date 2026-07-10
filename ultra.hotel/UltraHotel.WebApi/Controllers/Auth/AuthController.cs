using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UltraHotel.Application.Features.Auth.Commands;
using UltraHotel.Application.Features.Auth.Dtos;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.WebApi.Controllers.Auth;

/// <summary>Autenticación y gestión de usuarios.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
public class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Autentica un usuario y retorna un JWT.</summary>
    /// <response code="200">Token JWT, rol y userId.</response>
    /// <response code="400">Validación fallida.</response>
    /// <response code="401">Credenciales incorrectas.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        LoginResponse response = await sender.Send(command, ct);
        return Ok(response);
    }

    /// <summary>
    /// Registro público — crea únicamente viajeros (Traveler).
    /// El campo <c>role</c> del body es ignorado; siempre se asigna Traveler.
    /// </summary>
    /// <response code="201">Viajero registrado.</response>
    /// <response code="400">Validación fallida.</response>
    /// <response code="409">El email ya está registrado.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken ct)
    {
        RegisterCommand command = request with { Role = Role.Traveler };
        await sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Crea un agente (Agent). Solo accesible con rol ADMIN.
    /// Los agentes no pueden auto-crearse ni crear otros agentes.
    /// </summary>
    /// <response code="201">Agente creado.</response>
    /// <response code="400">Validación fallida.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">No tiene rol ADMIN.</response>
    /// <response code="409">El email ya está registrado.</response>
    [HttpPost("agents")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAgent([FromBody] RegisterCommand request, CancellationToken ct)
    {
        RegisterCommand command = request with { Role = Role.Agent };
        await sender.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created);
    }
}
