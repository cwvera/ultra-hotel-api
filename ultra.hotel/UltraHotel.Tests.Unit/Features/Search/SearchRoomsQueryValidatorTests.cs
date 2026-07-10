using FluentValidation.Results;
using UltraHotel.Application.Features.Search.Queries;

namespace UltraHotel.Tests.Unit.Features.Search;

public class SearchRoomsQueryValidatorTests
{
    private readonly SearchRoomsQueryValidator _validator = new();

    private static readonly DateOnly CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(1);
    private static readonly DateOnly CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(4);

    [Fact]
    public async Task Validate_ValidQuery_IsValid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new SearchRoomsQuery("bogotá", CheckIn, CheckOut, 2));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_EmptyCity_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new SearchRoomsQuery("", CheckIn, CheckOut, 2));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_CheckOutBeforeCheckIn_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new SearchRoomsQuery("bogotá", CheckOut, CheckIn, 2));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_ZeroGuests_IsInvalid()
    {
        ValidationResult result = await _validator.ValidateAsync(
            new SearchRoomsQuery("bogotá", CheckIn, CheckOut, 0));
        Assert.False(result.IsValid);
    }
}
