namespace EventBooking.Api.Dtos
{
    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;

        public DateTime StartsAt { get; set; }

        public string Address { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }
}
