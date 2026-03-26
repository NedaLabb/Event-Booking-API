namespace EventBooking.Api.Dtos
{
    public class EventResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime StartsAt { get; set; }

        public string Address { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public int BookingsCount { get; set; }
    }
}
