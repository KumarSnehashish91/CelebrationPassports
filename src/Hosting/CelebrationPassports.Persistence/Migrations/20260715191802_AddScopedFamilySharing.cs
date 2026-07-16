using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScopedFamilySharing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChapterContributors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterContributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterContributors_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChapterContributors_Users_InvitedBy",
                        column: x => x.InvitedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChapterContributors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChapterInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterInvitations_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChapterInvitations_Users_InvitedBy",
                        column: x => x.InvitedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterContributors_ChapterId_UserId",
                table: "ChapterContributors",
                columns: new[] { "ChapterId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChapterContributors_InvitedBy",
                table: "ChapterContributors",
                column: "InvitedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterContributors_UserId",
                table: "ChapterContributors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterInvitations_ChapterId_Status",
                table: "ChapterInvitations",
                columns: new[] { "ChapterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterInvitations_InvitedBy",
                table: "ChapterInvitations",
                column: "InvitedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterContributors");

            migrationBuilder.DropTable(
                name: "ChapterInvitations");
        }
    }
}
