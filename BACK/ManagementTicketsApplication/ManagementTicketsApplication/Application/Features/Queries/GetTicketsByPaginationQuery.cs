using ManagementTicketsApplication.Domain.Models;
using ManagementTicketsApplication.Data;
using Microsoft.EntityFrameworkCore;
using ManagementTicketsApplication.Domain.Enums;


namespace ManagementTicketsApplication.Application.Features.Queries
{
    /// <summary>
    /// Represents a query for retrieving tickets with pagination and filtering options.
    /// This class encapsulates the necessary data and logic to execute the retrieval of tickets 
    /// from the application's data context, supporting various filtering, sorting, and pagination criteria.
    /// </summary>
    public class GetTicketsByPaginationQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Description { get; set; }
        public TicketStatus? Status { get; set; }
        public long? Id { get; set; }
        public string SortBy { get; set; } = "date";
        public string SortOrder { get; set; } = "asc";

        public async Task<PagedResult<Ticket>> Execute(AppDbContext context)
        {
            var query = context.Tickets.AsQueryable();

            // Apply filters

            if (Id.HasValue)
            {
                query = query.Where(t => t.Id == Id.Value); 
            }

            if (!string.IsNullOrEmpty(Description))
            {
                query = query.Where(t => t.Description.Contains(Description));
            }

            if (Status.HasValue)
            {
                query = query.Where(t => t.Status == Status);
            }

            // Apply sorting
            query = SortTickets(query);

            var totalRecords = await query.CountAsync();
            var tickets = await query
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return new PagedResult<Ticket>(tickets, Page, totalRecords, PageSize);
        }

        private IQueryable<Ticket> SortTickets(IQueryable<Ticket> query)
        {
            return SortOrder.ToLower() == "desc" ? SortDescending(query) : SortAscending(query);
        }

        private IQueryable<Ticket> SortDescending(IQueryable<Ticket> query)
        {
            return SortBy.ToLower() switch
            {
                "id" => query.OrderByDescending(t => t.Id),
                "date" => query.OrderByDescending(t => t.Date),
                _ => query.OrderByDescending(t => t.Date)
            };
        }

        private IQueryable<Ticket> SortAscending(IQueryable<Ticket> query)
        {
            return SortBy.ToLower() switch
            {
                "id" => query.OrderBy(t => t.Id),
                "date" => query.OrderBy(t => t.Date),
                _ => query.OrderBy(t => t.Date)
            };
        }
    }
}
