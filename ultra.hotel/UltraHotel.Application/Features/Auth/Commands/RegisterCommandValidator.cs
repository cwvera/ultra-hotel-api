using FluentValidation;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Application.Features.Auth.Commands;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

        RuleFor(x => x.Role)
            .Must(r => r is Role.Agent or Role.Traveler)
            .WithMessage($"El rol debe ser {Role.Agent} o {Role.Traveler}.");
    }
}
