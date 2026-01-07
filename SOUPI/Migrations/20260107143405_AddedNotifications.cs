using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotifications : Migration
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
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NOTIFICATION",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICATION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_PROJECT_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PROJECT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_USER_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NOTIFICATION_USER_SenderId",
                        column: x => x.SenderId,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_Id",
                table: "NOTIFICATION",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_ProjectId",
                table: "NOTIFICATION",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_ReceiverId",
                table: "NOTIFICATION",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICATION_SenderId",
                table: "NOTIFICATION",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_CreatorId_Name",
                table: "PROJECT",
                columns: new[] { "CreatorId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_Id",
                table: "PROJECT",
                column: "Id",
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
                name: "NOTIFICATION");

            migrationBuilder.DropTable(
                name: "PROJECT");

            migrationBuilder.DropTable(
                name: "USER");
        }
    }
}
