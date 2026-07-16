using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChapterSoundtrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SongArtist",
                table: "Chapters",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SongLinkUrl",
                table: "Chapters",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SongTitle",
                table: "Chapters",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SongArtist",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "SongLinkUrl",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "SongTitle",
                table: "Chapters");
        }
    }
}
