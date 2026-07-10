using MediatR;
using UltraHotel.Application.Features.Hotels.Dtos;

namespace UltraHotel.Application.Features.Hotels.Queries;

/// <summary>Obtiene el resumen de reservas de un hotel.</summary>
public record GetHotelBookingsQuery(Guid HotelId) : IRequest<IReadOnlyList<HotelBookingSummaryDto>>;
