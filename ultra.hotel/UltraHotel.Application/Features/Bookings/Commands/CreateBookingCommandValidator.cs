using FluentValidation;

namespace UltraHotel.Application.Features.Bookings.Commands;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.CheckIn).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("La fecha de entrada no puede ser en el pasado.");
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn)
            .WithMessage("La fecha de salida debe ser posterior a la de entrada.");
        RuleFor(x => x.EmergencyContactName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EmergencyContactPhone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Guests).NotEmpty().WithMessage("Debe incluir al menos un huésped.");
        RuleForEach(x => x.Guests).ChildRules(g =>
        {
            g.RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            g.RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            g.RuleFor(x => x.Email).NotEmpty().EmailAddress();
            g.RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
            g.RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(50);
            g.RuleFor(x => x.Gender).IsInEnum();
            g.RuleFor(x => x.DocumentType).IsInEnum();
        });
    }
}
