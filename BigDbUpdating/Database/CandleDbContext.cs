using BigDbUpdating.Database;
using BigDbUpdating.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BigDbUpdating.Database
{
    public class CandleDbContext : DbContext, ICandleDbContext
    {
        public CandleDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CandleDbContext).Assembly);
        }

        public DbSet<Candle> Candles { get; protected set; }

        public DbSet<SplitCandleBackup> SplitCandleBackups { get; protected set; }
    }


    /// <inheritdoc />
    public class FirstContextFactory : IDesignTimeDbContextFactory<CandleDbContext>
    {
        /// <inheritdoc />
        public CandleDbContext CreateDbContext(string[] args)
        {
            var connectionString =
                "Host=localhost;Port=5432;Database=candles_test_db;Username=postgres;Password=postgres;Maximum Pool Size=1000";
            var optionsBuilder = new DbContextOptionsBuilder<CandleDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            var context = new CandleDbContext(optionsBuilder.Options);
            return context;
        }
    }
}