using ManagementTicketsApplication.Domain.Models;
using ManagementTicketsApplication.Domain.DTO;

namespace ManagementTicketsApplication.Domain.Mappers
{
    /// <summary>
    /// This class serves as a utility for transforming data 
    /// between domain models and data transfer objects (DTOs),
    /// enabling separation of concerns and promoting a clean architecture.
    /// </summary>
    public static class TicketMapper
    {
        public static TicketDto ToDto(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                Description = ticket.Description,
                Status = ticket.Status,
                Date = ticket.Date
            };
        }

        public static Ticket FromDto(TicketDto ticketDto)
        {
            return new Ticket
            {
                Id = ticketDto.Id,
                Description = ticketDto.Description,
                Status = ticketDto.Status,
                Date = ticketDto.Date
            };
        }
    }
}
