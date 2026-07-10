using FluentValidation.Results;
using UltraHotel.Application.Features.Bookings.Commands;
using UltraHotel.Application.Features.Bookings.Dtos;
using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Tests.Unit.Features.Bookings;

public class CreateBookingCommandValidatorTests
{
    private readonly CreateBookingCommandValidator _validator = new();

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    private static readonly DateOnly CheckIn = Today.AddDays(2);
    private static readonly DateOnly CheckOut = Today.AddDays(5);

    private static GuestRequest ValidGuest() => new(
        "Juan", "Pérez", new DateOnly(1990, 1, 1),
        Gender.Male, DocumentType.Passport, "AB123", "guest@test.com", "555-0001");

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateBookingCommand(Guid.NewGuid(), CheckIn, CheckOut, "Emergencia", "555-9999", [ValidGuest()]));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyRoomId_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateBookingCommand(Guid.Empty, CheckIn, CheckOut, "E", "555", [ValidGuest()]));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_CheckOutBeforeCheckIn_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateBookingCommand(Guid.NewGuid(), CheckOut, CheckIn, "E", "555", [ValidGuest()]));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyGuestList_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new CreateBookingCommand(Guid.NewGuid(), CheckIn, CheckOut, "E", "555", []));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_GuestWithInvalidEmail_IsInvalid()
    {
        GuestRequest badGuest = new("J", "P", new DateOnly(1990, 1, 1), Gender.Male, DocumentType.Passport, "DOC", "not-an-email", "555");
        ValidationResult result = await _validator.ValidateAsync(
            new CreateBookingCommand(Guid.NewGuid(), CheckIn, CheckOut, "E", "555", [badGuest]));
        Assert.False(result.IsValid);
    }
}
