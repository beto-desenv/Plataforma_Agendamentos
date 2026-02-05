using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma_Agendamentos.Constants;
using Plataforma_Agendamentos.Controllers;
using Plataforma_Agendamentos.Data;
using Plataforma_Agendamentos.DTOs;
using Plataforma_Agendamentos.Models;
using Xunit;

namespace Plataforma_Agendamentos.Tests;

public class BookingsControllerTests
{
    [Fact]
    public async Task CreateBooking_ReturnsBadRequest_WhenSlotAlreadyBooked()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        var client = new User
        {
            Id = Guid.NewGuid(),
            Name = "Cliente",
            Email = "cliente@example.com",
            PasswordHash = "hash",
            UserType = UserTypes.Cliente
        };

        var provider = new User
        {
            Id = Guid.NewGuid(),
            Name = "Prestador",
            Email = "prestador@example.com",
            PasswordHash = "hash",
            UserType = UserTypes.Prestador
        };

        var service = new Service
        {
            Id = Guid.NewGuid(),
            ProviderId = provider.Id,
            Title = "Consulta",
            Price = 120m,
            DurationMinutes = 60,
            Provider = provider
        };

        var bookingDate = DateTime.Now.AddDays(1).Date.AddHours(10);

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            ProviderId = provider.Id,
            DayOfWeek = (int)bookingDate.DayOfWeek,
            StartTime = bookingDate.TimeOfDay.Add(TimeSpan.FromHours(-1)),
            EndTime = bookingDate.TimeOfDay.Add(TimeSpan.FromHours(1)),
            Provider = provider
        };

        var existingBooking = new Booking
        {
            Id = Guid.NewGuid(),
            ClientId = client.Id,
            ServiceId = service.Id,
            Date = bookingDate,
            Status = BookingStatuses.Confirmado,
            Client = client,
            Service = service
        };

        context.AddRange(client, provider, service, schedule, existingBooking);
        context.SaveChanges();

        var controller = new BookingsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = BuildClaimsPrincipal(client.Id, UserTypes.Cliente)
                }
            }
        };

        var request = new BookingRequest
        {
            ServiceId = service.Id,
            Date = bookingDate
        };

        var result = await controller.CreateBooking(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Este horário já está ocupado.", badRequest.Value);
    }

    private static ClaimsPrincipal BuildClaimsPrincipal(Guid userId, string userType)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("UserType", userType)
        }, "Test");

        return new ClaimsPrincipal(identity);
    }
}
