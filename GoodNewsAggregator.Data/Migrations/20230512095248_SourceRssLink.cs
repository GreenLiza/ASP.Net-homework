using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodNewsAggregator.Data.Migrations
{
    /// <inheritdoc />
    public partial class SourceRssLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RssLink",
                table: "Sources",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RssLink",
                table: "Sources");
        }
    }
}
