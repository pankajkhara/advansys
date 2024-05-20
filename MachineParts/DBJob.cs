using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MachineParts
{
    public class DBJob
    {
        public DBJob(int id, DateTime startTime, DateTime endTime, string millName, string status)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
            MillName = millName;
            Status = status;
        }

        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MillName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // finished / cancelled
    }
    public class ApplicationDbContext : DbContext
    {
        public DbSet<DBJob> Jobs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            var databaseProvider = configuration["DatabaseProvider"];
            if (!string.IsNullOrEmpty(databaseProvider))
            {
                var connectionString = configuration.GetConnectionString(databaseProvider);

                if (databaseProvider == "SqlServer")
                {
                    optionsBuilder.UseSqlServer(connectionString);
                }
                else if (databaseProvider == "Sqlite")
                {
                     optionsBuilder.UseSqlite(connectionString);
                }
            }
        }
    }

}
