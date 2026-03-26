Event Booking API

RESTful API for managing events and seat bookings.

Features:
- Create, update, and delete events
- Book seats for events
- Prevent overbooking based on capacity
- View event availability and bookings

Technologies:
- ASP.NET Core
- Entity Framework Core
- SQL Server

Follow these steps to run the project locally:
1. Clone the repository:
```bash
git clone https://github.com/NedaLabb/Event-Booking-API.git
cd Event-Booking-API/EventBooking.Api/EventBooking.Api
```
2. Configure database:
Edit appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EventBookingDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

3. Apply migrations:
dotnet ef database update

4. Run the project:
dotnet run

5. Open Swagger:
Open in browser:
https://localhost:xxxx/swagger

Notes:
Make sure SQL Server is running.
Use Windows Authentication.
