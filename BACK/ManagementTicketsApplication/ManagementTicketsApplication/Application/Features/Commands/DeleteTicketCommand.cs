using ManagementTicketsApplication.Data;

namespace ManagementTicketsApplication.Application.Features.Commands
{
    /// <summary>
    /// Represents a command for deleting a ticket.
    /// This class encapsulates the necessary data and logic to execute the deletion of a ticket 
    /// from the application's data context based on its unique identifier.
    /// </summary>
    public class DeleteTicketCommand
    {
        public long Id { get; set; }

        public async Task<bool> Execute(AppDbContext context)
        {
            var ticket = await context.Tickets.FindAsync(Id);
            if (ticket == null) return false;

            context.Tickets.Remove(ticket);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
