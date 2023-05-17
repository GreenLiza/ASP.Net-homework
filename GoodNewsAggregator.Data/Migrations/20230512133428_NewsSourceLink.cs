using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodNewsAggregator.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewsSourceLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkToSource",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkToSource",
                table: "News");
        }
    }
}
