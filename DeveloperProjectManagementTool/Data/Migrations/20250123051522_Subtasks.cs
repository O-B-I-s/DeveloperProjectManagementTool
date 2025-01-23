using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeveloperProjectManagementTool.Data.Migrations
{
    /// <inheritdoc />
    public partial class Subtasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assignee",
                table: "SubTasks");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "SubTasks",
                newName: "IssueId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SubTasks",
                newName: "Description");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "SubTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "SubTasks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_IssueId",
                table: "SubTasks",
                column: "IssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_Issues_IssueId",
                table: "SubTasks",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTasks_Issues_IssueId",
                table: "SubTasks");

            migrationBuilder.DropIndex(
                name: "IX_SubTasks_IssueId",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "SubTasks");

            migrationBuilder.RenameColumn(
                name: "IssueId",
                table: "SubTasks",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "SubTasks",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Assignee",
                table: "SubTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
