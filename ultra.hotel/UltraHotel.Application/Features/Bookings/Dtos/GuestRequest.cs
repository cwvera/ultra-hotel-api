using UltraHotel.Domain.Entities.Bookings;

namespace UltraHotel.Application.Features.Bookings.Dtos;

/// <summary>Datos de un huésped al crear una reserva.</summary>
public record GuestRequest(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    Gender Gender,
    DocumentType DocumentType,
    string DocumentNumber,
    string Email,
    string Phone);
