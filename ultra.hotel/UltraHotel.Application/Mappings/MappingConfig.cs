using Mapster;
using UltraHotel.Application.Features.Auth.Commands;
using UltraHotel.Application.Features.Bookings.Dtos;
using UltraHotel.Application.Features.Hotels.Commands;
using UltraHotel.Application.Features.Hotels.Dtos;
using UltraHotel.Application.Features.Rooms.Dtos;
using UltraHotel.Domain.Entities.Bookings;
using UltraHotel.Domain.Entities.Hotels;
using UltraHotel.Domain.Entities.Identity;

namespace UltraHotel.Application.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Room, RoomDto>()
            .Map(dest => dest.RoomType, src => src.RoomType.ToString().ToUpper());

        config.NewConfig<Booking, BookingDto>()
            .Map(dest => dest.CheckIn, src => src.CheckInDate)
            .Map(dest => dest.CheckOut, src => src.CheckOutDate)
            .Map(dest => dest.Status, src => src.Status.ToString());

        config.NewConfig<Booking, HotelBookingSummaryDto>()
            .Map(dest => dest.BookingId, src => src.Id)
            .Map(dest => dest.CheckIn, src => src.CheckInDate)
            .Map(dest => dest.CheckOut, src => src.CheckOutDate)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.GuestCount, src => src.Guests.Count);

        config.NewConfig<Guest, GuestDto>()
            .Map(dest => dest.Gender, src => src.Gender.ToString())
            .Map(dest => dest.DocumentType, src => src.DocumentType.ToString());

        config.NewConfig<RegisterCommand, User>()
            .Map(dest => dest.Email, src => src.Email.ToLowerInvariant())
            .Ignore(dest => dest.PasswordHash);

        config.NewConfig<CreateHotelCommand, Hotel>()
            .Map(dest => dest.AgentEmail, src => src.AgentEmail.ToLowerInvariant());
    }
}
