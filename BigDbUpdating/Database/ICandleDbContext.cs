using BigDbUpdating.Database;
using BigDbUpdating.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BigDbUpdating.Database
{
    /// <summary>
    ///
    /// </summary>
    public interface ICandleDbContext : IAsyncDisposable, IDisposable
    {
        DbSet<Candle> Candles { get; }
        
        DbSet<SplitCandleBackup> SplitCandleBackups { get;}

        IModel Model { get; }

        ChangeTracker ChangeTracker { get; }

        /// <summary>
        /// База данных
        /// </summary>
        DatabaseFacade Database { get; }

        /// <summary>
        /// Сохранение
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}