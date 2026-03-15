using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOUPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedJobSequencesBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "JOB",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ParentJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JOB_JOB_ParentJobId",
                        column: x => x.ParentJobId,
                        principalTable: "JOB",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JOB_PROJECT_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PROJECT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JOB_USER_CreatorId",
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
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HasBeenViewed = table.Column<bool>(type: "bit", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
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

            migrationBuilder.CreateTable(
                name: "TEAMMEMBER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupervisorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEAMMEMBER", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TEAMMEMBER_PROJECT_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PROJECT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TEAMMEMBER_TEAMMEMBER_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "TEAMMEMBER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TEAMMEMBER_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JOBSEQUENCE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecondJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBSEQUENCE", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "ASSIGNMENT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
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
                name: "IX_ACTIVITY_AssignmentId",
                table: "ACTIVITY",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ACTIVITY_Id",
                table: "ACTIVITY",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_Id",
                table: "ASSIGNMENT",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_JobId",
                table: "ASSIGNMENT",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ASSIGNMENT_TeamMemberId_JobId",
                table: "ASSIGNMENT",
                columns: new[] { "TeamMemberId", "JobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JOB_CreatorId",
                table: "JOB",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_Id",
                table: "JOB",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JOB_ParentJobId",
                table: "JOB",
                column: "ParentJobId");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_ProjectId",
                table: "JOB",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_JOBSEQUENCE_FirstJobId_SecondJobId",
                table: "JOBSEQUENCE",
                columns: new[] { "FirstJobId", "SecondJobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JOBSEQUENCE_SecondJobId",
                table: "JOBSEQUENCE",
                column: "SecondJobId");

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
                name: "IX_TEAMMEMBER_Id",
                table: "TEAMMEMBER",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_ProjectId",
                table: "TEAMMEMBER",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_SupervisorId",
                table: "TEAMMEMBER",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_TEAMMEMBER_UserId_ProjectId",
                table: "TEAMMEMBER",
                columns: new[] { "UserId", "ProjectId" },
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
                name: "ACTIVITY");

            migrationBuilder.DropTable(
                name: "JOBSEQUENCE");

            migrationBuilder.DropTable(
                name: "NOTIFICATION");

            migrationBuilder.DropTable(
                name: "ASSIGNMENT");

            migrationBuilder.DropTable(
                name: "JOB");

            migrationBuilder.DropTable(
                name: "TEAMMEMBER");

            migrationBuilder.DropTable(
                name: "PROJECT");

            migrationBuilder.DropTable(
                name: "USER");
        }
    }
}
