using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedAssignmentsTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TEAMMEMBER_TEAMMEMBER_SupervisorUserId_SupervisorProjectId",
                table: "TEAMMEMBER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TEAMMEMBER",
                table: "TEAMMEMBER");

            migrationBuilder.DropIndex(
                name: "IX_TEAMMEMBER_SupervisorUserId_SupervisorProjectId",
                table: "TEAMMEMBER");

            migrationBuilder.DropColumn(
                name: "SupervisorProjectId",
                table: "TEAMMEMBER");

            migrationBuilder.RenameColumn(
                name: "SupervisorUserId",
                table: "TEAMMEMBER",
                newName: "SupervisorId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TEAMMEMBER",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TEAMMEMBER",
                table: "TEAMMEMBER",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ASSIGNMENT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASSIGNMENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ASSIGNMENT_JOB_JobId",
                        column: x => x.JobId,
                        principalTable: "JOB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ASSIGNMENT_TEAMMEMBER_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TEAMMEMBER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_SupervisorId",
                table: "TEAMMEMBER",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_JobId",
                table: "ASSIGNMENT",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_TeamMemberId_JobId",
                table: "ASSIGNMENT",
                columns: new[] { "TeamMemberId", "JobId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TEAMMEMBER_TEAMMEMBER_SupervisorId",
                table: "TEAMMEMBER",
                column: "SupervisorId",
                principalTable: "TEAMMEMBER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TEAMMEMBER_TEAMMEMBER_SupervisorId",
                table: "TEAMMEMBER");

            migrationBuilder.DropTable(
                name: "ASSIGNMENT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TEAMMEMBER",
                table: "TEAMMEMBER");

            migrationBuilder.DropIndex(
                name: "IX_TEAMMEMBER_SupervisorId",
                table: "TEAMMEMBER");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TEAMMEMBER");

            migrationBuilder.RenameColumn(
                name: "SupervisorId",
                table: "TEAMMEMBER",
                newName: "SupervisorUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "SupervisorProjectId",
                table: "TEAMMEMBER",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TEAMMEMBER",
                table: "TEAMMEMBER",
                columns: new[] { "UserId", "ProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_SupervisorUserId_SupervisorProjectId",
                table: "TEAMMEMBER",
                columns: new[] { "SupervisorUserId", "SupervisorProjectId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TEAMMEMBER_TEAMMEMBER_SupervisorUserId_SupervisorProjectId",
                table: "TEAMMEMBER",
                columns: new[] { "SupervisorUserId", "SupervisorProjectId" },
                principalTable: "TEAMMEMBER",
                principalColumns: new[] { "UserId", "ProjectId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
