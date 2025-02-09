using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeveloperProjectManagementTool.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssueId",
                table: "ProjectHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SprintId",
                table: "ProjectHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubTaskId",
                table: "ProjectHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProjectHistories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistories_IssueId",
                table: "ProjectHistories",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistories_SprintId",
                table: "ProjectHistories",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistories_SubTaskId",
                table: "ProjectHistories",
                column: "SubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHistories_UserId",
                table: "ProjectHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHistories_AspNetUsers_UserId",
                table: "ProjectHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHistories_Issues_IssueId",
                table: "ProjectHistories",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHistories_Sprints_SprintId",
                table: "ProjectHistories",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHistories_SubTasks_SubTaskId",
                table: "ProjectHistories",
                column: "SubTaskId",
                principalTable: "SubTasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHistories_AspNetUsers_UserId",
                table: "ProjectHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHistories_Issues_IssueId",
                table: "ProjectHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHistories_Sprints_SprintId",
                table: "ProjectHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHistories_SubTasks_SubTaskId",
                table: "ProjectHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectHistories_IssueId",
                table: "ProjectHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectHistories_SprintId",
                table: "ProjectHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectHistories_SubTaskId",
                table: "ProjectHistories");

            migrationBuilder.DropIndex(
                name: "IX_ProjectHistories_UserId",
                table: "ProjectHistories");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "ProjectHistories");

            migrationBuilder.DropColumn(
                name: "SprintId",
                table: "ProjectHistories");

            migrationBuilder.DropColumn(
                name: "SubTaskId",
                table: "ProjectHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectHistories");
        }
    }
}
