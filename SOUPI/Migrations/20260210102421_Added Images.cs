using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "PROJECT");

            migrationBuilder.CreateTable(
                name: "IMAGE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMAGE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IMAGE_PROJECT_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PROJECT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IMAGE_Id",
                table: "IMAGE",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IMAGE_ParentId",
                table: "IMAGE",
                column: "ParentId",
                unique: true,
                filter: "[ParentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IMAGE");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "PROJECT",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
