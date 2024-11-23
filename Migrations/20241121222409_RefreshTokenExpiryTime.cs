using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OloEcomm.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenExpiryTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshTokenExpiryTime" },
                values: new object[] { "08fcd6fb-767c-4e7d-ba23-d162c5a2ba99", "AQAAAAIAAYagAAAAEB5cYKpAmjbhHcNg6Bb++J7CCT2N1RSLZErp2psijyfBB1QsrGwJ5Ivb8yrmK7nbfQ==", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "89d3ec57-32d1-4f0c-9f5a-88bf1f46a08b", "AQAAAAIAAYagAAAAEDczYxnb5B39dF+fypIaSSlGX+cZrgvsTULGGHlS6wycNLWy179I9GAgLC5NTVltmQ==" });
        }
    }
}
