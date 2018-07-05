using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum020.Server.Migrations
{
    public partial class addeduseridtopoststoallowforpostandfiledeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserIdentifier",
                table: "Posts",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIdentifier",
                table: "Posts");
        }
    }
}
