using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwentyQuestions.Migrations
{
    /// <inheritdoc />
    public partial class RenameQuestionIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Question",
                newName: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "Question",
                newName: "Id");
        }
    }
}
