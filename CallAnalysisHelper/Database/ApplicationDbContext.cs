using CallAnalysisHelper.Models;
using Microsoft.EntityFrameworkCore;

namespace CallAnalysisHelper.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CallRecord> CallRecords { get; set; }
        public DbSet<Client> Clients { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }

}
