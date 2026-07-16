using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestbookSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuestbookSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuestName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedMediaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestbookSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestbookSubmissions_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuestbookSubmissions_Media_ApprovedMediaId",
                        column: x => x.ApprovedMediaId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GuestbookSubmissions_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuestbookSubmissions_ApprovedMediaId",
                table: "GuestbookSubmissions",
                column: "ApprovedMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestbookSubmissions_ChapterId_Status",
                table: "GuestbookSubmissions",
                columns: new[] { "ChapterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_GuestbookSubmissions_ReviewedByUserId",
                table: "GuestbookSubmissions",
                column: "ReviewedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestbookSubmissions");
        }
    }
}
