using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum020.Server.Migrations
{
    public partial class uppeddefaultmaxthreadcountto12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MaximumThreadCount",
                table: "Config",
                nullable: false,
                defaultValue: 12,
                oldClrType: typeof(int),
                oldDefaultValue: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MaximumThreadCount",
                table: "Config",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(int),
                oldDefaultValue: 12);
        }
    }
}
