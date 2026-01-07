using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedJobSequences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JOBSEQUENCE");
        }
    }
}
