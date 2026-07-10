using MediatR;

namespace UltraHotel.Application.Features.Hotels.Commands;

/// <summary>Habilita o deshabilita un hotel.</summary>
public record ToggleHotelStatusCommand(Guid HotelId, bool IsEnabled) : IRequest;
