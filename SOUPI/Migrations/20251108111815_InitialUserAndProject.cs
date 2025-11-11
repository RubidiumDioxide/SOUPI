using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserAndProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PROJECT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GithubRepository = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROJECT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PROJECT_USER_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "USER",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_CreatorId",
                table: "PROJECT",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_Id",
                table: "PROJECT",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_Name",
                table: "PROJECT",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER_Id",
                table: "USER",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER_Login",
                table: "USER",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PROJECT");

            migrationBuilder.DropTable(
                name: "USER");
        }
    }
}
