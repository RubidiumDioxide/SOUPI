using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ASSIGNMENT_TeamMemberId_JobId",
                table: "ASSIGNMENT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PROJECT",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "JOB",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ASSIGNMENT",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ACTIVITY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Commit = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACTIVITY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ACTIVITY_ASSIGNMENT_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "ASSIGNMENT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_Id",
                table: "TEAMMEMBER",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_Id",
                table: "ASSIGNMENT",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_TeamMemberId_JobId",
                table: "ASSIGNMENT",
                columns: new[] { "TeamMemberId", "JobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ACTIVITY_AssignmentId",
                table: "ACTIVITY",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ACTIVITY_Id",
                table: "ACTIVITY",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ACTIVITY");

            migrationBuilder.DropIndex(
                name: "IX_TEAMMEMBER_Id",
                table: "TEAMMEMBER");

            migrationBuilder.DropIndex(
                name: "IX_ASSIGNMENT_Id",
                table: "ASSIGNMENT");

            migrationBuilder.DropIndex(
                name: "IX_ASSIGNMENT_TeamMemberId_JobId",
                table: "ASSIGNMENT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PROJECT",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "JOB",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ASSIGNMENT",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_TeamMemberId_JobId",
                table: "ASSIGNMENT",
                columns: new[] { "TeamMemberId", "JobId" });
        }
    }
}
