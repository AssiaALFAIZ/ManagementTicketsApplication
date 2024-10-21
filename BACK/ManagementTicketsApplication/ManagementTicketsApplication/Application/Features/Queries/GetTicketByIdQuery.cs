using ManagementTicketsApplication.Domain.Models;
using ManagementTicketsApplication.Data;

namespace ManagementTicketsApplication.Application.Features.Queries
{
    /// <summary>
    /// Represents a query for retrieving a ticket by its unique identifier.
    /// This class encapsulates the necessary data and logic to execute the retrieval of a ticket 
    /// from the application's data context.
    /// </summary>
    public class GetTicketByIdQuery
    {
        public long Id { get; set; }

        public async Task<Ticket> Execute(AppDbContext context)
        {
            return await context.Tickets.FindAsync(Id);
        }
    }
}
