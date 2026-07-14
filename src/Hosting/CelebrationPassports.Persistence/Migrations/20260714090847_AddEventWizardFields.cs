using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventWizardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Places",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Places",
                type: "numeric(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Places",
                type: "numeric(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Places",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Places",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CoverMediaId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Events",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventType",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllDay",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Events",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Events",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CoverMediaId",
                table: "Events",
                column: "CoverMediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Media_CoverMediaId",
                table: "Events",
                column: "CoverMediaId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Media_CoverMediaId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CoverMediaId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "CoverMediaId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsAllDay",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Events");
        }
    }
}
