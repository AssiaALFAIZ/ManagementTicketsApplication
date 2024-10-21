using ManagementTicketsApplication.Domain.Enums;

namespace ManagementTicketsApplication.Domain.Models
{
    /// <summary>
    /// Represents a ticket in the management application.
    /// This class contains the properties that define a ticket,
    /// including its unique identifier, description, status, and date.
    /// </summary>
    public class Ticket
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

    }
}
