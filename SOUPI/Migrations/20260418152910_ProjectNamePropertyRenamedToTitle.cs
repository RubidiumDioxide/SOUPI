using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class ProjectNamePropertyRenamedToTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDateTime",
                table: "PROJECT");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PROJECT",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_PROJECT_CreatorId_Name",
                table: "PROJECT",
                newName: "IX_PROJECT_CreatorId_Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "PROJECT",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_PROJECT_CreatorId_Title",
                table: "PROJECT",
                newName: "IX_PROJECT_CreatorId_Name");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateTime",
                table: "PROJECT",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
