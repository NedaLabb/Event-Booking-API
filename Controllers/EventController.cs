using EventBooking.Api.Models;
using Microsoft.AspNetCore.Mvc;
using EventBooking.Api.Data;
using Microsoft.EntityFrameworkCore;
using EventBooking.Api.Dtos;


namespace EventBooking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly AppDbContext _db;

    public EventController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventResponse>>> GetAll()
    {
        var events = await _db.Events
        .Include(e => e.Bookings)
        .ToListAsync();

        var response = events.Select(ev => new EventResponse
        {
            Id = ev.Id,
            Title = ev.Title,
            StartsAt = ev.StartsAt,
            Address = ev.Address,
            Capacity = ev.Capacity,
            BookingsCount = ev.Bookings.Count
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponse>> GetById(int id)
    {
        var ev = await _db.Events
        .Include(e => e.Bookings)
        .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
        {
            return NotFound();
        }

        var response = new EventResponse
        {
            Id = ev.Id,
            Title = ev.Title,
            StartsAt = ev.StartsAt,
            Address = ev.Address,
            Capacity = ev.Capacity,
            BookingsCount = ev.Bookings.Count
        };

        return Ok(response);
    }

    [HttpGet("{id:int}/availability")]
    public async Task<ActionResult<object>> GetAvailability(int id)
    {
        var ev = await _db.Events.Include(e => e.Bookings).FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
        {
            return NotFound("Event not found.");
        }

        var reservedSeats = ev.Bookings.Sum(b => b.ReservedSeats);
        var availableSeats = ev.Capacity - reservedSeats;

        return Ok(new
        {
            eventId = ev.Id,
            title = ev.Title,
            capacity = ev.Capacity,
            reservedSeats = reservedSeats,
            availableSeats = availableSeats
        });
    }

    [HttpGet("{id:int}/bookings")]
    public async Task<ActionResult<List<BookingResponse>>> GetBookingsForEvent(int id)
    {
        var ev = await _db.Events
            .Include(e => e.Bookings)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound("Event not found.");

        var response = ev.Bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            EventId = b.EventId,
            ReservedSeats = b.ReservedSeats,
            ReservationTime = b.ReservationTime
        }).ToList();

        return Ok(response);
    }



    [HttpPost]
    public async Task<ActionResult<EventResponse>> Create(CreateEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required.");
        }
        if (request.Capacity <= 0)
        {
            return BadRequest("Capacity must be greater than 0.");
        }
        if (request.StartsAt <= DateTime.UtcNow)
        {
            return BadRequest("Event must be in the future.");
        }

        var newEvent = new Event
        {
            Title = request.Title,
            StartsAt = request.StartsAt,
            Address = request.Address,
            Capacity = request.Capacity
        };

        _db.Events.Add(newEvent);
        await _db.SaveChangesAsync();

        var response = new EventResponse
        {
            Id = newEvent.Id,
            Title = newEvent.Title,
            StartsAt = newEvent.StartsAt,
            Address = newEvent.Address,
            Capacity = newEvent.Capacity,
            BookingsCount = 0
        };

        return CreatedAtAction(nameof(GetById), new { id = newEvent.Id }, response);
    }

    [HttpPost("{id:int}/book")]
    public async Task<ActionResult<BookingResponse>> BookEvent(int id, CreateBookingRequest request)
    {
        var ev = await _db.Events
            .Include(e => e.Bookings)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound("Event not found.");

        if (request.ReservedSeats <= 0)
            return BadRequest("Reserved seats must be more than 0.");

        var alreadyReservedSeats = ev.Bookings.Sum(b => b.ReservedSeats);
        var availableSeats = ev.Capacity - alreadyReservedSeats;

        if (request.ReservedSeats > availableSeats)
            return BadRequest($"Not enough seats available. Only {availableSeats} left.");

        var booking = new Booking
        {
            EventId = ev.Id,
            ReservedSeats = request.ReservedSeats
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        var response = new BookingResponse
        {
            Id = booking.Id,
            EventId = booking.EventId,
            ReservedSeats = booking.ReservedSeats,
            ReservationTime = booking.ReservationTime
        };

        return Ok(response);
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEventRequest request)
    {
        var existing = await _db.Events.FindAsync(id);

        if (existing == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        if (request.Capacity <= 0)
            return BadRequest("Capacity must be greater than 0.");

        if (request.StartsAt <= DateTime.UtcNow)
            return BadRequest("Event must be in the future.");

        existing.Title = request.Title;
        existing.StartsAt = request.StartsAt;
        existing.Address = request.Address;
        existing.Capacity = request.Capacity;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _db.Events.FindAsync(id);

        if (ev == null)
            return NotFound();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
