using ManagementTicketsApplication.Domain.Models;
using ManagementTicketsApplication.Data;

namespace ManagementTicketsApplication.Application.Features.Commands
{
    /// <summary>
    /// Represents a command for updating an existing ticket.
    /// This class encapsulates the necessary data and logic to execute the update of a ticket 
    /// within the application's data context based on its unique identifier.
    /// </summary>
    public class UpdateTicketCommand
    {
        public long Id { get; set; }
        public Ticket UpdatedTicket { get; set; }

        public async Task<bool> Execute(AppDbContext context)
        {
            var existingTicket = await context.Tickets.FindAsync(Id);
            if (existingTicket == null) return false;

            existingTicket.Description = UpdatedTicket.Description;
            existingTicket.Status = UpdatedTicket.Status;
            existingTicket.Date = UpdatedTicket.Date;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
