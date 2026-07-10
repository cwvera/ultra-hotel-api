using MediatR;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Application.Features.Auth.Commands;

/// <summary>Comando para registrar un nuevo usuario en el sistema.</summary>
/// <param name="Email">Correo electrónico único del usuario.</param>
/// <param name="Password">Contraseña en texto plano (mín. 6 caracteres).</param>
/// <param name="Role">Rol asignado: <c>Agent</c> o <c>Traveler</c>.</param>
public record RegisterCommand(string Email, string Password, Role Role) : IRequest;
