using ManagementTicketsApplication.Domain.Models;
using ManagementTicketsApplication.Data;

namespace ManagementTicketsApplication.Application.Features.Commands
{
    /// <summary>
    /// Represents a command for creating a new ticket.
    /// This class encapsulates the necessary data and logic to execute the creation of a ticket 
    /// within the application's data context.
    /// </summary>
    public class CreateTicketCommand
    {
        public Ticket NewTicket { get; set; }

        public async Task<Ticket> Execute(AppDbContext context)
        {
            context.Tickets.Add(NewTicket);
            await context.SaveChangesAsync();
            return NewTicket;
        }
    }
}
