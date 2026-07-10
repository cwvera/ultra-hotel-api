using Mapster;
using UltraHotel.Application.Features.Search.Dtos;
using UltraHotel.Domain.Entities.Hotels;

namespace UltraHotel.Application.Mappings;

internal static class RoomIndexMapper
{
    private static readonly TypeAdapterConfig Config = BuildConfig();

    private static TypeAdapterConfig BuildConfig()
    {
        TypeAdapterConfig config = new();
        config.NewConfig<(Room room, Hotel hotel), RoomIndexDocument>()
            .Map(dest => dest.RoomId, src => src.room.Id.ToString())
            .Map(dest => dest.HotelId, src => src.hotel.Id.ToString())
            .Map(dest => dest.HotelName, src => src.hotel.Name)
            .Map(dest => dest.City, src => src.hotel.City.ToLower())
            .Map(dest => dest.RoomType, src => src.room.RoomType.ToString().ToUpper())
            .Map(dest => dest.Capacity, src => src.room.Capacity)
            .Map(dest => dest.BasePrice, src => (double)src.room.BasePrice)
            .Map(dest => dest.TaxRate, src => (double)src.room.TaxRate)
            .Map(dest => dest.LocationInHotel, src => src.room.LocationInHotel)
            .Map(dest => dest.IsAvailable, _ => true)
            .Map(dest => dest.HotelEnabled, src => src.hotel.IsEnabled)
            .Map(dest => dest.RoomEnabled, src => src.room.IsEnabled)
            .Map(dest => dest.YearMonth, _ => DateTime.UtcNow.ToString("yyyyMM"));
        return config;
    }

    internal static RoomIndexDocument ToIndexDocument(this Room room, Hotel hotel) =>
        (room, hotel).Adapt<RoomIndexDocument>(Config);
}
