using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updatedentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PassportInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PassportId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassportInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassportInvitations_Passports_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PassportInvitations_Users_InvitedBy",
                        column: x => x.InvitedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PassportOwnershipHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PassportId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TransferredBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassportOwnershipHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassportOwnershipHistory_Passports_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PassportOwnershipHistory_Users_NewOwnerId",
                        column: x => x.NewOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PassportOwnershipHistory_Users_OldOwnerId",
                        column: x => x.OldOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PassportOwnershipHistory_Users_TransferredBy",
                        column: x => x.TransferredBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PassportPeople",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PassportId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassportPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassportPeople_Passports_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PassportPeople_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PassportPeople_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PassportShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PassportId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShareToken = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassportShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassportShares_Passports_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Country = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PassportId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trips_Passports_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trips_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trips_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PassportId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    TripId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoverMediaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chapters_Passports_PassportId",
                        column: x => x.PassportId,
                        principalTable: "Passports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chapters_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chapters_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chapters_Users_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CategoryId",
                table: "Chapters",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_DeletedBy",
                table: "Chapters",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_PassportId_EventDate",
                table: "Chapters",
                columns: new[] { "PassportId", "EventDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_PlaceId",
                table: "Chapters",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_TripId",
                table: "Chapters",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportInvitations_InvitedBy",
                table: "PassportInvitations",
                column: "InvitedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PassportInvitations_PassportId",
                table: "PassportInvitations",
                column: "PassportId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportOwnershipHistory_NewOwnerId",
                table: "PassportOwnershipHistory",
                column: "NewOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportOwnershipHistory_OldOwnerId",
                table: "PassportOwnershipHistory",
                column: "OldOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportOwnershipHistory_PassportId",
                table: "PassportOwnershipHistory",
                column: "PassportId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportOwnershipHistory_TransferredBy",
                table: "PassportOwnershipHistory",
                column: "TransferredBy");

            migrationBuilder.CreateIndex(
                name: "IX_PassportPeople_DeletedBy",
                table: "PassportPeople",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PassportPeople_PassportId",
                table: "PassportPeople",
                column: "PassportId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportPeople_UserId",
                table: "PassportPeople",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportShares_PassportId",
                table: "PassportShares",
                column: "PassportId");

            migrationBuilder.CreateIndex(
                name: "IX_PassportShares_ShareToken",
                table: "PassportShares",
                column: "ShareToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DeletedBy",
                table: "Trips",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_PassportId",
                table: "Trips",
                column: "PassportId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_PlaceId",
                table: "Trips",
                column: "PlaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "PassportInvitations");

            migrationBuilder.DropTable(
                name: "PassportOwnershipHistory");

            migrationBuilder.DropTable(
                name: "PassportPeople");

            migrationBuilder.DropTable(
                name: "PassportShares");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Places");
        }
    }
}
