using MediatR;
using UltraHotel.Application.Features.Auth.Dtos;

namespace UltraHotel.Application.Features.Auth.Commands;

/// <summary>Comando para autenticar un usuario y obtener un JWT.</summary>
/// <param name="Email">Correo electrónico registrado.</param>
/// <param name="Password">Contraseña en texto plano.</param>
public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
