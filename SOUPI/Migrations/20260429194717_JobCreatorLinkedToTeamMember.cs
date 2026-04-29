using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class JobCreatorLinkedToTeamMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JOB_USER_CreatorId",
                table: "JOB");

            migrationBuilder.AddForeignKey(
                name: "FK_JOB_TEAMMEMBER_CreatorId",
                table: "JOB",
                column: "CreatorId",
                principalTable: "TEAMMEMBER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JOB_TEAMMEMBER_CreatorId",
                table: "JOB");

            migrationBuilder.AddForeignKey(
                name: "FK_JOB_USER_CreatorId",
                table: "JOB",
                column: "CreatorId",
                principalTable: "USER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
