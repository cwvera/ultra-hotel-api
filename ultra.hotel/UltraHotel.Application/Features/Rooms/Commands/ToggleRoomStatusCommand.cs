using MediatR;

namespace UltraHotel.Application.Features.Rooms.Commands;

/// <summary>Habilita o deshabilita una habitación.</summary>
public record ToggleRoomStatusCommand(Guid RoomId, bool IsEnabled) : IRequest;
