using Mapster;
using MediatR;
using UltraHotel.Application.Features.Auth.Contracts;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Application.Features.Auth.Commands;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand>
{
    /// <inheritdoc />
    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("El email ya está registrado.");
        }

        User user = request.Adapt<User>();
        user.PasswordHash = passwordHasher.Hash(request.Password);
        await userRepository.AddAsync(user, cancellationToken);
    }
}
