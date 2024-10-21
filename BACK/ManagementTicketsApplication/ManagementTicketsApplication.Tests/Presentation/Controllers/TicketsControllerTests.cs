using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using ManagementTicketsApplication.Data;
using ManagementTicketsApplication.Domain.Enums;
using ManagementTicketsApplication.Domain.Models;
using ManagementTicketsApplication.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xunit;

public class TicketsControllerTests
{
    private AppDbContext _context;
    private readonly TicketsController _controller;

    public TicketsControllerTests()
    {
        // Create a mock configuration object
        var configurationMock = new Mock<IConfiguration>();
        // Directly return a connection string
        configurationMock.Setup(c => c["ConnectionStrings:WebApiDatabase"]).Returns("YourConnectionStringHere");

        // Use a unique database name for each test
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Pass the mock configuration to the AppDbContext
        _context = new AppDbContext(options, configurationMock.Object);
        _controller = new TicketsController(_context);
    }

    [Fact]
    public async Task CreateTicket_ShouldReturnCreatedAtAction_WhenTicketIsCreated()
    {
        // Arrange
        var newTicket = new Ticket { Id = 3L, Description = "New Ticket", Status = TicketStatus.Open };

        // Act
        var result = await _controller.CreateTicket(newTicket);

        // Assert
        var createdResult = result.Should().BeOfType<ActionResult<Ticket>>().Which;
        createdResult.Result.Should().BeOfType<CreatedAtActionResult>();
        var ticket = ((CreatedAtActionResult)createdResult.Result).Value.Should().BeEquivalentTo(newTicket);

        // Verify the ticket was added to the context
        var savedTicket = await _context.Tickets.FindAsync(3L);
        savedTicket.Should().NotBeNull();
        savedTicket.Description.Should().Be("New Ticket");
    }

    [Fact]
    public async Task UpdateTicket_ShouldReturnNoContent_WhenTicketIsUpdated()
    {
        // Arrange
        var existingTicket = new Ticket { Id = 1L, Description = "Existing Ticket", Status = TicketStatus.Open };
        await _context.Tickets.AddAsync(existingTicket);
        await _context.SaveChangesAsync();

        var updatedTicket = new Ticket { Id = 1L, Description = "Updated Ticket", Status = TicketStatus.Closed };

        // Act
        var result = await _controller.UpdateTicket(1L, updatedTicket);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify the ticket was updated
        var ticket = await _context.Tickets.FindAsync(1L);
        ticket.Description.Should().Be("Updated Ticket");
        ticket.Status.Should().Be(TicketStatus.Closed);
    }

    [Fact]
    public async Task GetTicketById_ShouldReturnOkResult_WhenTicketExists()
    {
        // Arrange
        var existingTicket = new Ticket { Id = 1L, Description = "Existing Ticket", Status = TicketStatus.Open };
        await _context.Tickets.AddAsync(existingTicket);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetTicketById(1L);

        // Assert
        var actionResult = result.Should().BeOfType<ActionResult<Ticket>>().Which;
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var ticket = ((OkObjectResult)actionResult.Result).Value.Should().BeEquivalentTo(existingTicket);
    }

    [Fact]
    public async Task GetTickets_ShouldReturnOkResult_WhenTicketsExist()
    {
        // Arrange
        _context.Tickets.Add(new Ticket { Id = 1L, Description = "Test Ticket 1", Status = TicketStatus.Open });
        _context.Tickets.Add(new Ticket { Id = 2L, Description = "Test Ticket 2", Status = TicketStatus.Closed });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetTickets();

        // Assert
        var actionResult = result.Should().BeOfType<ActionResult<PagedResult<Ticket>>>().Which;
        actionResult.Result.Should().BeOfType<OkObjectResult>();
        var pagedResult = (actionResult.Result as OkObjectResult)?.Value as PagedResult<Ticket>;
        pagedResult.Should().NotBeNull();
        pagedResult.Records.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteTicket_ShouldReturnNoContent_WhenTicketIsDeleted()
    {
        // Arrange
        var ticketToDelete = new Ticket { Id = 1L, Description = "Delete Ticket", Status = TicketStatus.Open };
        await _context.Tickets.AddAsync(ticketToDelete);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteTicket(1L);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify the ticket was deleted
        var deletedTicket = await _context.Tickets.FindAsync(1L);
        deletedTicket.Should().BeNull();
    }
}
