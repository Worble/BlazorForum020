using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum020.Server.Migrations
{
    public partial class makedeletebehaviourcascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_ThreadId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_ThreadId",
                table: "Posts",
                column: "ThreadId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_ThreadId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_ThreadId",
                table: "Posts",
                column: "ThreadId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
