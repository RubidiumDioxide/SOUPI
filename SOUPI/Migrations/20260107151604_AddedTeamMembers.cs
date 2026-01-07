using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedTeamMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TEAMMEMBER",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupervisorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupervisorProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEAMMEMBER", x => new { x.UserId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_TEAMMEMBER_PROJECT_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PROJECT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TEAMMEMBER_TEAMMEMBER_SupervisorUserId_SupervisorProjectId",
                        columns: x => new { x.SupervisorUserId, x.SupervisorProjectId },
                        principalTable: "TEAMMEMBER",
                        principalColumns: new[] { "UserId", "ProjectId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TEAMMEMBER_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_ProjectId",
                table: "TEAMMEMBER",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_SupervisorUserId_SupervisorProjectId",
                table: "TEAMMEMBER",
                columns: new[] { "SupervisorUserId", "SupervisorProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_UserId_ProjectId",
                table: "TEAMMEMBER",
                columns: new[] { "UserId", "ProjectId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TEAMMEMBER");
        }
    }
}
