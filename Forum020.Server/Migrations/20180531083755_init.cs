using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum020.Server.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateEdited = table.Column<DateTime>(nullable: true),
                    BoardId = table.Column<int>(nullable: false),
                    MaximumThreadCount = table.Column<int>(nullable: false, defaultValue: 10),
                    MaximumReplyCount = table.Column<int>(nullable: false, defaultValue: 100)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateEdited = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NameShort = table.Column<string>(nullable: false),
                    ConfigId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boards_Config_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "Config",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateEdited = table.Column<DateTime>(nullable: true),
                    BoardId = table.Column<int>(nullable: false),
                    ThreadId = table.Column<int>(nullable: true),
                    IdEffective = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    IsOp = table.Column<bool>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    BumpDate = table.Column<DateTime>(nullable: true),
                    IsArchived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_ConfigId",
                table: "Boards",
                column: "ConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boards_Name",
                table: "Boards",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boards_NameShort",
                table: "Boards",
                column: "NameShort",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BoardId",
                table: "Posts",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ThreadId",
                table: "Posts",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IdEffective_BoardId",
                table: "Posts",
                columns: new[] { "IdEffective", "BoardId" },
                unique: true);

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.make_board_seq()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                begin
                  execute format('create sequence board_seq_%s', NEW.""Id"");
                  return NEW;
                            end
                $function$");

            migrationBuilder.Sql(@"
                CREATE TRIGGER make_board_seq AFTER INSERT ON ""Boards"" FOR EACH ROW EXECUTE PROCEDURE make_board_seq();");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.fill_in_post_seq()
                 RETURNS trigger
                 LANGUAGE plpgsql
                AS $function$
                begin
                  NEW.""IdEffective"" := nextval('board_seq_' || NEW.""BoardId"");
                  RETURN NEW;
                            end
                $function$");

            migrationBuilder.Sql(@"
                CREATE TRIGGER fill_in_post_seq BEFORE INSERT ON ""Posts"" FOR EACH ROW EXECUTE PROCEDURE fill_in_post_seq();");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropTable(
                name: "Config");
        }
    }
}
