using ManagementTicketsApplication.Application.Features.Commands;
using ManagementTicketsApplication.Application.Features.Queries;
using ManagementTicketsApplication.Data;
using ManagementTicketsApplication.Domain.Enums;
using ManagementTicketsApplication.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManagementTicketsApplication.Presentation.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to ticket management.
    /// Provides endpoints for creating, updating, deleting, 
    /// and retrieving tickets, as well as retrieving tickets 
    /// with pagination and filtering options.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        //Retrieves a paginated list of tickets with optional filtering by description, status, and ID.

        [HttpGet]
        public async Task<ActionResult<PagedResult<Ticket>>> GetTickets(
            int page = 1,
            int pageSize = 10,
            string description = null,
            TicketStatus? status = null,
             long? id = null,
            string sortBy = "date",
            string sortOrder = "asc")
        {
            var query = new GetTicketsByPaginationQuery
            {
                Page = page,
                PageSize = pageSize,
                Description = description,
                Status = status,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Id = id
            };

            var result = await query.Execute(_context);
            return Ok(result);
        }

        // Creates a new ticket and saves it to the database.

        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket([FromBody] Ticket newTicket)
        {
            var command = new CreateTicketCommand { NewTicket = newTicket };
            await command.Execute(_context);

            return CreatedAtAction(nameof(GetTickets), new { id = newTicket.Id }, newTicket);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTicket(long id, [FromBody] Ticket updatedTicket)
        {
            var command = new UpdateTicketCommand { Id = id, UpdatedTicket = updatedTicket };
            var result = await command.Execute(_context);
            return result ? NoContent() : NotFound();
        }

        //Updates an existing ticket identified by ID with new data.

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTicket(long id)
        {
            var command = new DeleteTicketCommand { Id = id };
            var result = await command.Execute(_context);
            return result ? NoContent() : NotFound();
        }

        // Deletes a ticket identified by ID from the database.
    
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(long id)
        {
            var query = new GetTicketByIdQuery { Id = id };
            var ticket = await query.Execute(_context);
            return ticket != null ? Ok(ticket) : NotFound();
        }
    }
}
