namespace UltraHotel.Application.Features.Bookings.Dtos;

public record GuestDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Gender,
    string DocumentType,
    string DocumentNumber,
    string Email,
    string Phone);
