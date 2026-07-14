using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CelebrationPassports.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorySeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Icon", "Name" },
                values: new object[,]
                {
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000001"), "bi-airplane", "Travel" },
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000002"), "bi-people", "Family" },
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000003"), "bi-balloon-heart", "Celebration" },
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000004"), "bi-cup-straw", "Food" },
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000005"), "bi-people-fill", "Friends" },
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000006"), "bi-trophy", "Milestone" },
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000007"), "bi-compass", "Adventure" },
                    { new Guid("9a1b7e10-0001-4a00-8000-000000000008"), "bi-sun", "Everyday" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9a1b7e10-0001-4a00-8000-000000000008"));
        }
    }
}
