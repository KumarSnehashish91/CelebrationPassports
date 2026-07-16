using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftStories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiftDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecipientEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PassportTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PersonalMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PrintFormat = table.Column<int>(type: "integer", nullable: true),
                    PassportGiftId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftDrafts_PassportGifts_PassportGiftId",
                        column: x => x.PassportGiftId,
                        principalTable: "PassportGifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GiftDrafts_Users_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedStories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GiftDraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OpeningParagraph = table.Column<string>(type: "text", nullable: false),
                    ClosingParagraph = table.Column<string>(type: "text", nullable: false),
                    PullQuoteText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PullQuoteOrigin = table.Column<int>(type: "integer", nullable: true),
                    BodyParagraphsJson = table.Column<string>(type: "text", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RegenerationCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedStories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedStories_GiftDrafts_GiftDraftId",
                        column: x => x.GiftDraftId,
                        principalTable: "GiftDrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiftPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GiftDraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserInsight = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AiGeneratedInsight = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftPhotos_GiftDrafts_GiftDraftId",
                        column: x => x.GiftDraftId,
                        principalTable: "GiftDrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedStories_GiftDraftId",
                table: "GeneratedStories",
                column: "GiftDraftId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GiftDrafts_PassportGiftId",
                table: "GiftDrafts",
                column: "PassportGiftId",
                unique: true,
                filter: "\"PassportGiftId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GiftDrafts_SenderUserId",
                table: "GiftDrafts",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftPhotos_GiftDraftId_DisplayOrder",
                table: "GiftPhotos",
                columns: new[] { "GiftDraftId", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedStories");

            migrationBuilder.DropTable(
                name: "GiftPhotos");

            migrationBuilder.DropTable(
                name: "GiftDrafts");
        }
    }
}
