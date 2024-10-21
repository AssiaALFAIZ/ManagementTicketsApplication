using ManagementTicketsApplication.Domain.Enums;

namespace ManagementTicketsApplication.Domain.DTO
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for a ticket.
    /// This class is used to transfer ticket data between layers of the application,
    /// particularly when interacting with the presentation layer or APIs.
    /// </summary>
    public class TicketDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime Date { get; set; }
    }
}
