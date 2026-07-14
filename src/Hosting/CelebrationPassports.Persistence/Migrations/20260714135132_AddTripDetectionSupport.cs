using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTripDetectionSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HomePlaceId",
                table: "UserProfiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CapturedAt",
                table: "Media",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Media",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Media",
                type: "numeric(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StoryId",
                table: "Chapters",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "PassportId",
                table: "Chapters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Chapters",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Chapters",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            // PassportId defaulted to Guid.Empty above for any pre-existing rows (this
            // column didn't exist before) — backfill it from the parent Story before the
            // FK constraint below is added, or that constraint would fail immediately for
            // any Chapter created prior to this migration.
            migrationBuilder.Sql(
                """
                UPDATE "Chapters" c
                SET "PassportId" = s."PassportId"
                FROM "Stories" s
                WHERE c."StoryId" = s."Id";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_HomePlaceId",
                table: "UserProfiles",
                column: "HomePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_PassportId",
                table: "Chapters",
                column: "PassportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Passports_PassportId",
                table: "Chapters",
                column: "PassportId",
                principalTable: "Passports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Places_HomePlaceId",
                table: "UserProfiles",
                column: "HomePlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Passports_PassportId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Places_HomePlaceId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_HomePlaceId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_PassportId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "HomePlaceId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CapturedAt",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "PassportId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Chapters");

            migrationBuilder.AlterColumn<Guid>(
                name: "StoryId",
                table: "Chapters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
