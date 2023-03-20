// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Globalization;
using System.Text;
using BigDbUpdating.Database;
using BigDbUpdating.Database.Entities;
using BigDbUpdating.Database.Settings;
using BigDbUpdating.Exctensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
var services = new ServiceCollection();

services.TryAddSingleton<ILoggerFactory>(new SerilogLoggerFactory());
services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));


services.Configure<DbSettings>(settings =>
{
    settings.Host = "localhost";
    settings.Password = "postgres";
    settings.User = "postgres";
    settings.Port = 5432;
    settings.DbName = "candles_test_db";
    settings.MaxPoolSize = 1000;
    settings.NoResetOnClose = true;
});

services.AddDbContext<ICandleDbContext, CandleDbContext>((provider, options) =>
{
    var dbSettings = provider.GetRequiredService<IOptions<DbSettings>>().Value;
    options.UseNpgsql(dbSettings.GetConnectionString());
});


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
await using var provider = services.BuildServiceProvider();
var dbContext = provider.GetRequiredService<ICandleDbContext>();
var logger = provider.GetRequiredService<ILogger<Program>>();

await InitializeCandles(dbContext);
// await InitializeBackup(dbContext);
// await ButchUpdate(dbContext);
await ButchRollback(dbContext);
logger.LogInformation("Hello, World!");


async Task InitializeCandles(ICandleDbContext context)
{
    var random = new Random();
    var start = new DateTime(2010, 1, 1);

    context.ChangeTracker.AutoDetectChangesEnabled = false;
    logger.LogInformation("Start initialize candles");
    while (await context.Candles.CountAsync() < 1000000)
    {
        logger.LogInformation("Starting 100k initialization");
        var candles = new List<Candle>();
        for (var i = 0; i < 100000; i++)
        {
            var candleDate = start.AddMinutes(1);
            candles.Add(new Candle
            {
                ClassCode = "TQBR",
                SecurityCode = "SBER",
                High = random.Next(1, 3000),
                Low = random.Next(1, 3000),
                Close = random.Next(1, 3000),
                Open = random.Next(1, 3000),
                Quantity = random.Next(5000, 200000),
                QuantityPieces = random.Next(5000, 20000),
                Volume = random.Next(1, 3000),
                TradeDateTime = candleDate
            });
            start = candleDate;
        }

        context.Candles.AddRange(candles);
        await context.SaveChangesAsync();
        logger.LogInformation("Added 100k candles");
    }

    context.ChangeTracker.AutoDetectChangesEnabled = true;
    logger.LogInformation("Initialization ended");
}
//
//
// async Task InitializeBackup(ICandleDbContext context)
// {
//     var id = new Guid("bdf79a8d-fc13-4780-a5ad-4009e5343e1f");
//     if (context.SplitCandleBackups.Any(a => a.SplitId == id))
//         return;
//
//     var date = DateTime.UtcNow.Date;
//     var backup = await context.Candles.AsNoTracking().Select(a => new SplitCandleBackup
//     {
//         ClassCode = a.ClassCode, SecurityCode = a.SecurityCode, Close = a.Close, Open = a.Open, High = a.High, Low = a.Low,
//         Quantity = a.Quantity, QuantityPieces = a.QuantityPieces, TradeDateTime = a.TradeDateTime, SplitTradeDate = date, SplitId = id
//     }).Distinct().ToListAsync();
//
//     context.ChangeTracker.AutoDetectChangesEnabled = false;
//     logger.LogInformation("Start initialize backup");
//     foreach (var splitCandleBackup in backup)
//     {
//         context.SplitCandleBackups.Add(splitCandleBackup);
//     }
//
//     await context.SaveChangesAsync();
//
//     context.ChangeTracker.AutoDetectChangesEnabled = true;
//     logger.LogInformation("Backup initialization ended");
// }

async Task ButchUpdate(ICandleDbContext context)
{
    var earliestCandle = await context.Candles
        .Where(a => a.ClassCode == "TQBR" && a.SecurityCode == "SBER")
        .OrderBy(a => a.TradeDateTime)
        .FirstOrDefaultAsync();
    if (earliestCandle == null)
        throw new ArgumentException();

    var range = GetDividedDatesRange(earliestCandle.TradeDateTime);
    await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
    try
    {
        for (var i = 0; i < range.Count; i++)
        {
            var dates = range[i];
            var query = BuildQuery(context, dates.Item1, dates.Item2);
            await context.Database.ExecuteSqlRawAsync(query);

            logger.LogInformation("Part of split finished, from {Start} to {End}", dates.Item1, dates.Item2);
            // await Task.Delay(TimeSpan.FromSeconds(1));
        }

        await transaction.CommitAsync();
        logger.LogInformation("All parts of split finished successfully");
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        logger.LogError(ex, "Unexpected error in split");
        throw;
    }
}

async Task ButchRollback(ICandleDbContext context)
{
    var id = new Guid("bdf79a8d-fc13-4780-a5ad-4009e5343e1f");

    var query = BuildRollbackQuery(context, DateTime.UtcNow.Date, id);
}

IDictionary<int, (DateTime, DateTime)> GetDividedDatesRange(DateTime startDate)
{
    // Отступ, чтобы не пропустить первую свечу
    startDate = startDate.Add(-TimeSpan.FromMinutes(1));
    // в среднем 700 минуток в торговый день для интсрументов
    // поэтому, один батч 21000 шт
    var dayChunkSize = 30;
    var endDate = DateTime.UtcNow.Date;
    var result = new Dictionary<int, (DateTime, DateTime)>();
    var index = 0;
    DateTime chunkEnd;

    while ((chunkEnd = startDate.AddDays(dayChunkSize)) < endDate)
    {
        result.Add(index, (startDate, chunkEnd));
        startDate = chunkEnd;
        index++;
    }

    result.Add(index, (startDate, endDate));

    return result;
}

string BuildQuery(ICandleDbContext context, DateTime startDate, DateTime endDate)
{
    var numerator = 2;
    var denominator = 1;
    var classCodeValue = "TQBR";
    var securityCodeValue = "SBER";

    var candleMetadata = context.Model.FindEntityType(typeof(Candle)) ?? throw new AggregateException();
    var openProperty = candleMetadata.FindProperty(nameof(Candle.Open));
    var closeProperty = candleMetadata.FindProperty(nameof(Candle.Close));
    var highProperty = candleMetadata.FindProperty(nameof(Candle.High));
    var lowProperty = candleMetadata.FindProperty(nameof(Candle.Low));
    var quantityProperty = candleMetadata.FindProperty(nameof(Candle.Quantity));
    var piecesProperty = candleMetadata.FindProperty(nameof(Candle.QuantityPieces));
    var securityCodeProperty = candleMetadata.FindProperty(nameof(Candle.SecurityCode));
    var classCodeProperty = candleMetadata.FindProperty(nameof(Candle.ClassCode));
    var tradeDateProperty = candleMetadata.FindProperty(nameof(Candle.TradeDateTime));

    var open = openProperty!.GetColumnName(StoreObjectIdentifier.Table(openProperty!.DeclaringEntityType.GetTableName()!));
    var close = closeProperty!.GetColumnName(StoreObjectIdentifier.Table(closeProperty!.DeclaringEntityType.GetTableName()!));
    var high = highProperty!.GetColumnName(StoreObjectIdentifier.Table(highProperty!.DeclaringEntityType.GetTableName()!));
    var low = lowProperty!.GetColumnName(StoreObjectIdentifier.Table(lowProperty!.DeclaringEntityType.GetTableName()!));
    var quantity =
        quantityProperty!.GetColumnName(StoreObjectIdentifier.Table(quantityProperty!.DeclaringEntityType.GetTableName()!));
    var quantityPieces =
        piecesProperty!.GetColumnName(StoreObjectIdentifier.Table(piecesProperty!.DeclaringEntityType.GetTableName()!));

    var securityCode =
        securityCodeProperty!.GetColumnName(
            StoreObjectIdentifier.Table(securityCodeProperty!.DeclaringEntityType.GetTableName()!));
    var classCode =
        classCodeProperty!.GetColumnName(StoreObjectIdentifier.Table(classCodeProperty!.DeclaringEntityType.GetTableName()!));
    var tradeDateTime =
        tradeDateProperty!.GetColumnName(StoreObjectIdentifier.Table(tradeDateProperty!.DeclaringEntityType.GetTableName()!));


    var query = new StringBuilder($"UPDATE {candleMetadata.GetTableName()} SET ")
        .Append($"{open}={open}*{numerator}/{denominator}, ")
        .Append($"{close}={close}*{numerator}/{denominator}, ")
        .Append($"{high}={high}*{numerator}/{denominator}, ")
        .Append($"{low}={low}*{numerator}/{denominator}, ")
        .Append($"{quantity}={quantity}/{numerator}*{denominator}, ")
        .Append($"{quantityPieces}={quantityPieces}/{numerator}*{denominator}")
        .AppendLine()
        .Append($"WHERE {securityCode}='{securityCodeValue}' ")
        .Append("AND ")
        .Append($"{classCode}='{classCodeValue}' ")
        .Append("AND ")
        .Append($"{tradeDateTime} > {ToPgString(startDate)}")
        .Append("AND ")
        .Append($"{tradeDateTime} <= {ToPgString(endDate)};").ToString();

    return query;
}

string BuildRollbackQuery(ICandleDbContext context, DateTime endDate, Guid splitId)
{
    var candleMetadata = context.Model.FindEntityType(typeof(Candle)) ?? throw new AggregateException();
    var backupMetadata = context.Model.FindEntityType(typeof(SplitCandleBackup)) ?? throw new AggregateException();
    var m1BackupTableName = backupMetadata.GetTableName();
    var m1CandlesTableName = candleMetadata.GetTableName();
    var m1BackupColumnNames = backupMetadata.GetProperties()
        .Where(o => o != backupMetadata.FindProperty(nameof(SplitCandleBackup.SplitId)) &&
                    o != backupMetadata.FindProperty(nameof(SplitCandleBackup.SplitTradeDate))
        ).Select(o => o.GetColumnName(StoreObjectIdentifier.Table(o.DeclaringEntityType.GetTableName()!)))
        .OrderBy(a => a).ToArray();

    var m1KeyColumnNames = candleMetadata.FindPrimaryKey()!.Properties
        .Select(o => o.GetColumnName(StoreObjectIdentifier.Table(o.DeclaringEntityType.GetTableName()!))).ToArray();

    var m1CandlesColumnNames = candleMetadata.GetProperties()
        .Select(o => o.GetColumnName(StoreObjectIdentifier.Table(o.DeclaringEntityType.GetTableName()!))).OrderBy(a => a).ToArray();

    if (m1BackupColumnNames.Length != m1CandlesColumnNames.Length)
        throw new InvalidOperationException("Not equal count of base properties.");

    var splitIdProperty = backupMetadata.FindProperty(nameof(SplitCandleBackup.SplitId))
                          ?? throw new ArgumentNullException(nameof(Program), "BuildParamsForM1Recover went wrong");
    var splitTradeDateProperty = backupMetadata.FindProperty(nameof(SplitCandleBackup.SplitTradeDate))
                                 ?? throw new ArgumentNullException(
                                     nameof(Program), "BuildParamsForM1Recover went wrong");

    var splitIdColumnName =
        splitIdProperty.GetColumnName(StoreObjectIdentifier.Table(splitIdProperty.DeclaringEntityType.GetTableName()!));
    var splitDateColumnName =
        splitTradeDateProperty.GetColumnName(
            StoreObjectIdentifier.Table(splitTradeDateProperty.DeclaringEntityType.GetTableName()!));

    var query = new StringBuilder($"INSERT INTO {m1CandlesTableName} ")
        .Append("(").AppendJoin(',', m1CandlesColumnNames).Append(")")
        .AppendLine()
        .Append("SELECT ")
        .AppendJoin(',', m1BackupColumnNames)
        .AppendLine()
        .Append($"FROM {m1BackupTableName}")
        .AppendLine()
        .Append(
            $"WHERE {splitDateColumnName}>={ToPgString(endDate)} AND {splitDateColumnName}<={ToPgString(endDate)}")
        .Append($"AND {splitIdColumnName}={ToPgString(splitId)}")
        .AppendLine()
        .Append($"ON CONFLICT ")
        .Append("(").AppendJoin(',', m1KeyColumnNames).Append(") DO UPDATE")
        .AppendLine()
        .Append("SET")
        .AppendLine()
        .AppendJoin(',', m1BackupColumnNames.Where(o => !m1KeyColumnNames.Contains(o)).Select(o => $"{o}=excluded.{o}"))
        .Append(';').ToString();

    return query;
}

static string? ToPgString(object param)
{
    return param switch
    {
        string strParam => $"\'{strParam}\'",
        int intParam => intParam.ToString(),
        long longParam => longParam.ToString(),
        Guid guidParam => $"\'{guidParam}\'",
        decimal decimalParam => decimalParam.ToString(CultureInfo.InvariantCulture),
        DateTime dateTimeParam => $"\'{dateTimeParam.ToUniversalTime():O}\'",
        _ => param.ToString()
    };
}