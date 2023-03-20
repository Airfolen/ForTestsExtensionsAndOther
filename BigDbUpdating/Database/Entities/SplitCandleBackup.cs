namespace BigDbUpdating.Database.Entities;

public class SplitCandleBackup : IEquatable<SplitCandleBackup>
{
    /// <summary>
    /// Split id
    /// </summary>
    public Guid SplitId { get; set; }

    /// <summary>
    /// Дата сплита только для партиционирования
    /// </summary>
    public DateTime SplitTradeDate { get; set; }

    /// <summary>
    ///     Время открытия свечи
    /// </summary>
    public DateTime TradeDateTime { get; set; }

    /// <summary>
    ///     Код биржи
    /// </summary>
    public string ClassCode { get; set; }

    /// <summary>
    ///     Код инструмента
    /// </summary>
    public string SecurityCode { get; set; }

    /// <summary>
    ///     Цена открытия
    /// </summary>
    public decimal Open { get; set; }

    /// <summary>
    ///     Цена закрытия
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    ///     Максимальная цена
    /// </summary>
    public decimal High { get; set; }

    /// <summary>
    ///     Минимальная цена
    /// </summary>
    public decimal Low { get; set; }

    /// <summary>
    ///     Объем торгов
    /// </summary>
    public decimal Volume { get; set; }

    /// <summary>
    ///     Количество по лотам
    /// </summary>
    public long Quantity { get; set; }

    /// <summary>
    ///     Количество по единицам
    /// </summary>
    public long QuantityPieces { get; set; }

    public bool Equals(SplitCandleBackup? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SplitId.Equals(other.SplitId) && SplitTradeDate.Equals(other.SplitTradeDate) && TradeDateTime.Equals(other.TradeDateTime) && ClassCode == other.ClassCode && SecurityCode == other.SecurityCode;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SplitCandleBackup) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SplitId, SplitTradeDate, TradeDateTime, ClassCode, SecurityCode);
    }
}