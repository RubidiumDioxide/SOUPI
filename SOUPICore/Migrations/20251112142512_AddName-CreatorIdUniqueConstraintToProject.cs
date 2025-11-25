using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPICore.Migrations
{
    /// <inheritdoc />
    public partial class AddNameCreatorIdUniqueConstraintToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PROJECT_Name",
                table: "PROJECT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_Name",
                table: "PROJECT",
                column: "Name",
                unique: true);
        }
    }
}
