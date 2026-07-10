using MediatR;
using UltraHotel.Application.Features.Auth.Contracts;
using UltraHotel.Application.Features.Auth.Dtos;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Application.Features.Auth.Commands;

public class LoginCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    /// <inheritdoc />
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User user = await userRepository.FindByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        return new LoginResponse(tokenService.GenerateToken(user), user.Role.ToString().ToUpper(), user.Id);
    }
}
