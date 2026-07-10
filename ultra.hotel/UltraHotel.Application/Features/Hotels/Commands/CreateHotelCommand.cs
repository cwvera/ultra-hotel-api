using MediatR;
using UltraHotel.Application.Features.Hotels.Dtos;

namespace UltraHotel.Application.Features.Hotels.Commands;

/// <summary>Crea un nuevo hotel. Solo accesible por Agentes.</summary>
public record CreateHotelCommand(
    string Name,
    string City,
    string Address,
    string? Description,
    string AgentEmail) : IRequest<HotelDto>;
