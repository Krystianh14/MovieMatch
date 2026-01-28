using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieMatch.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKindToUserTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "UserTitles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "UserTitles");
        }
    }
}
