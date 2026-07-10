using FluentValidation;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Features.Rooms.Commands;

public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.RoomType).IsInEnum();
        RuleFor(x => x.BasePrice).GreaterThan(0).WithMessage("El precio base debe ser mayor a 0.");
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 1).WithMessage("La tasa de impuesto debe estar entre 0 y 1 (ej: 0.19 = 19%).");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("La capacidad debe ser al menos 1.");
        RuleFor(x => x.LocationInHotel).MaximumLength(200).When(x => x.LocationInHotel is not null);
    }
}
