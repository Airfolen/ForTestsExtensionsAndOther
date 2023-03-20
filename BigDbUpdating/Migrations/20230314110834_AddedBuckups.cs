using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigDbUpdating.Migrations
{
    public partial class AddedBuckups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "m1",
                columns: table => new
                {
                    trade_datetime = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    class_code = table.Column<string>(type: "varchar(12)", nullable: false),
                    security_code = table.Column<string>(type: "varchar(12)", nullable: false),
                    open = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    close = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    high = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    low = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    volume = table.Column<decimal>(type: "numeric(36,8)", nullable: false),
                    quantity = table.Column<long>(type: "bigint", nullable: false),
                    quantity_pieces = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("m1_id", x => new { x.class_code, x.security_code, x.trade_datetime });
                });

            migrationBuilder.CreateTable(
                name: "split_m1_backup",
                columns: table => new
                {
                    split_id = table.Column<Guid>(type: "uuid", nullable: false),
                    split_datetime = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    trade_datetime = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    class_code = table.Column<string>(type: "varchar(12)", nullable: false),
                    security_code = table.Column<string>(type: "varchar(12)", nullable: false),
                    open = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    close = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    high = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    low = table.Column<decimal>(type: "numeric(19,8)", nullable: false),
                    volume = table.Column<decimal>(type: "numeric(36,8)", nullable: false),
                    quantity = table.Column<long>(type: "bigint", nullable: false),
                    quantity_pieces = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("split_m1_id", x => new { x.split_datetime, x.split_id, x.class_code, x.security_code, x.trade_datetime });
                });

            migrationBuilder.CreateIndex(
                name: "split_m1_idx",
                table: "split_m1_backup",
                column: "split_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "m1");

            migrationBuilder.DropTable(
                name: "split_m1_backup");
        }
    }
}
