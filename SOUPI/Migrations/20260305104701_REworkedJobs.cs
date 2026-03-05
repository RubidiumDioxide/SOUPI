using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class REworkedJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOBSEQUENCE");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "JOB");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "JOB",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "NextJobId",
                table: "JOB",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "JOB",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateTime",
                table: "JOB",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_JOB_NextJobId",
                table: "JOB",
                column: "NextJobId",
                unique: true,
                filter: "[NextJobId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_JOB_JOB_NextJobId",
                table: "JOB",
                column: "NextJobId",
                principalTable: "JOB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JOB_JOB_NextJobId",
                table: "JOB");

            migrationBuilder.DropIndex(
                name: "IX_JOB_NextJobId",
                table: "JOB");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "JOB");

            migrationBuilder.DropColumn(
                name: "NextJobId",
                table: "JOB");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "JOB");

            migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "JOB");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "JOB",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JOBSEQUENCE",
                columns: table => new
                {
                    FirstJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecondJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBSEQUENCE", x => new { x.FirstJobId, x.SecondJobId });
                    table.ForeignKey(
                        name: "FK_JOBSEQUENCE_JOB_FirstJobId",
                        column: x => x.FirstJobId,
                        principalTable: "JOB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JOBSEQUENCE_JOB_SecondJobId",
                        column: x => x.SecondJobId,
                        principalTable: "JOB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JOBSEQUENCE_FirstJobId_SecondJobId",
                table: "JOBSEQUENCE",
                columns: new[] { "FirstJobId", "SecondJobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JOBSEQUENCE_SecondJobId",
                table: "JOBSEQUENCE",
                column: "SecondJobId");
        }
    }
}
