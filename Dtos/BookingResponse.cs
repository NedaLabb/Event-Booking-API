namespace EventBooking.Api.Dtos;

public class BookingResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int EventId { get; set; }
    public int ReservedSeats { get; set; }
    public DateTime ReservationTime { get; set; }
}
