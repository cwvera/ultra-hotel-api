using System.Text.Json.Serialization;

namespace UltraHotel.Application.Features.Search.Dtos;

/// <summary>Documento indexado en Elasticsearch para cada habitación.</summary>
public class RoomIndexDocument
{
    [JsonPropertyName("hotel_id")]    public string HotelId { get; set; } = string.Empty;
    [JsonPropertyName("hotel_name")]  public string HotelName { get; set; } = string.Empty;
    [JsonPropertyName("city")]        public string City { get; set; } = string.Empty;

    [JsonPropertyName("room_id")]          public string RoomId { get; set; } = string.Empty;
    [JsonPropertyName("room_type")]        public string RoomType { get; set; } = string.Empty;
    [JsonPropertyName("capacity")]         public int Capacity { get; set; }
    [JsonPropertyName("base_price")]       public double BasePrice { get; set; }
    [JsonPropertyName("tax_rate")]         public double TaxRate { get; set; }
    [JsonPropertyName("location_in_hotel")]public string? LocationInHotel { get; set; }

    [JsonPropertyName("is_available")]  public bool IsAvailable { get; set; } = true;
    [JsonPropertyName("hotel_enabled")] public bool HotelEnabled { get; set; } = true;
    [JsonPropertyName("room_enabled")]  public bool RoomEnabled { get; set; } = true;

    [JsonPropertyName("year_month")] public string YearMonth { get; set; } = DateTime.UtcNow.ToString("yyyyMM");
}
