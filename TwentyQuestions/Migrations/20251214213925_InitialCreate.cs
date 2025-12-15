using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwentyQuestions.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CharacterName = table.Column<string>(type: "TEXT", nullable: false),
                    AnswererUserName = table.Column<string>(type: "TEXT", nullable: false),
                    AskerUserName = table.Column<string>(type: "TEXT", nullable: true),
                    IsOver = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsWon = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuestionText = table.Column<string>(type: "TEXT", nullable: false),
                    Answer = table.Column<int>(type: "INTEGER", nullable: true),
                    GameStateId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_Games_GameStateId",
                        column: x => x.GameStateId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_GameStateId",
                table: "Question",
                column: "GameStateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
