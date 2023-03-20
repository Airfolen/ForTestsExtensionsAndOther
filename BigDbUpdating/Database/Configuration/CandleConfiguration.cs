
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigDbUpdating.Database;

public class CandleConfiguration : IEntityTypeConfiguration<Candle>
{
    public void Configure(EntityTypeBuilder<Candle> builder)
    {
        builder.HasKey(x => new
            {
                x.ClassCode,
                x.SecurityCode,
                x.TradeDateTime
            })
            .HasName("m1_id");

        builder.ToTable("m1");

        builder.Property(x => x.TradeDateTime)
            .HasColumnType("timestamptz")
            .HasColumnName("trade_datetime");

        builder.Property(x => x.ClassCode)
            .HasColumnType("varchar(12)")
            .HasColumnName("class_code");

        builder.Property(x => x.SecurityCode)
            .HasColumnType("varchar(12)")
            .HasColumnName("security_code");

        builder.Property(x => x.Open)
            .HasColumnType("numeric(19,8)")
            .HasColumnName("open");

        builder.Property(x => x.Close)
            .HasColumnType("numeric(19,8)")
            .HasColumnName("close");

        builder.Property(x => x.High)
            .HasColumnType("numeric(19,8)")
            .HasColumnName("high");

        builder.Property(x => x.Low)
            .HasColumnType("numeric(19,8)")
            .HasColumnName("low");

        builder.Property(x => x.Volume)
            .HasColumnType("numeric(36,8)")
            .HasColumnName("volume");

        builder.Property(x => x.Quantity)
            .HasColumnType("bigint")
            .HasColumnName("quantity");

        builder.Property(x => x.QuantityPieces)
            .HasColumnType("bigint")
            .HasColumnName("quantity_pieces");
    }
}