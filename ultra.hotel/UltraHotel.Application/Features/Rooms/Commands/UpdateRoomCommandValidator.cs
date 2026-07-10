using FluentValidation;

namespace UltraHotel.Application.Features.Rooms.Commands;

public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.RoomType).IsInEnum();
        RuleFor(x => x.BasePrice).GreaterThan(0);
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 1);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.LocationInHotel).MaximumLength(200).When(x => x.LocationInHotel is not null);
    }
}
