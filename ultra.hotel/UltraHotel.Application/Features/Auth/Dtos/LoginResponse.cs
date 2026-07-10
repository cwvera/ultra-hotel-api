namespace UltraHotel.Application.Features.Auth.Dtos;

public record LoginResponse(string Token, string Role, Guid UserId);
