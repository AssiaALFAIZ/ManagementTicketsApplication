using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ManagementTicketsApplication.Domain.Models;

namespace ManagementTicketsApplication.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Constructor to pass IConfiguration
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Configure the DbContext if not already configured
            if (!options.IsConfigured)
            {
                // Use PostgreSQL connection string from the configuration
                options.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
            }
        }

        public DbSet<Ticket> Tickets { get; set; }
    }
}
