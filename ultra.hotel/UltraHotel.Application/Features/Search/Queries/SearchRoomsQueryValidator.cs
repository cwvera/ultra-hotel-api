using FluentValidation;

namespace UltraHotel.Application.Features.Search.Queries;

public class SearchRoomsQueryValidator : AbstractValidator<SearchRoomsQuery>
{
    public SearchRoomsQueryValidator()
    {
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CheckIn).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("La fecha de entrada no puede ser en el pasado.");
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn)
            .WithMessage("La fecha de salida debe ser posterior a la de entrada.");
        RuleFor(x => x.Guests).GreaterThan(0).WithMessage("El número de huéspedes debe ser al menos 1.");
    }
}
