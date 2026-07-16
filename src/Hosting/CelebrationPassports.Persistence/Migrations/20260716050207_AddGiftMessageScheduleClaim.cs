using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftMessageScheduleClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryMode",
                table: "PassportGifts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MessageMediaUrl",
                table: "PassportGifts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "PassportGifts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDeliveryDate",
                table: "PassportGifts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryMode",
                table: "GiftDrafts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MessageMediaUrl",
                table: "GiftDrafts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "GiftDrafts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDeliveryDate",
                table: "GiftDrafts",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryMode",
                table: "PassportGifts");

            migrationBuilder.DropColumn(
                name: "MessageMediaUrl",
                table: "PassportGifts");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "PassportGifts");

            migrationBuilder.DropColumn(
                name: "ScheduledDeliveryDate",
                table: "PassportGifts");

            migrationBuilder.DropColumn(
                name: "DeliveryMode",
                table: "GiftDrafts");

            migrationBuilder.DropColumn(
                name: "MessageMediaUrl",
                table: "GiftDrafts");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "GiftDrafts");

            migrationBuilder.DropColumn(
                name: "ScheduledDeliveryDate",
                table: "GiftDrafts");
        }
    }
}
