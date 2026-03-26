using System.Text.Json.Serialization;

namespace EventBooking.Api.Models;

public class Booking
{
    public int Id { get; set; }

    public int EventId { get; set; }

    [JsonIgnore] 
    public Event Event { get; set; } = null!;

    public int ReservedSeats { get; set; }

    public DateTime ReservationTime { get; set; } = DateTime.UtcNow;
}
