namespace BigDbUpdating.Database;

public class Candle
{
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
}