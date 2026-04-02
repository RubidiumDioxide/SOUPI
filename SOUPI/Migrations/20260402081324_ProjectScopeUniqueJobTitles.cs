using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class ProjectScopeUniqueJobTitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JOB_ProjectId",
                table: "JOB");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_ProjectId_Title",
                table: "JOB",
                columns: new[] { "ProjectId", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JOB_ProjectId_Title",
                table: "JOB");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_ProjectId",
                table: "JOB",
                column: "ProjectId");
        }
    }
}
