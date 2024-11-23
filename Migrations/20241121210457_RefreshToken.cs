using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OloEcomm.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshToken" },
                values: new object[] { "89d3ec57-32d1-4f0c-9f5a-88bf1f46a08b", "AQAAAAIAAYagAAAAEDczYxnb5B39dF+fypIaSSlGX+cZrgvsTULGGHlS6wycNLWy179I9GAgLC5NTVltmQ==", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "22c2cbe6-55c8-43d8-b437-e54717f866ca", "AQAAAAIAAYagAAAAEGXKaAxp2OCL/XQE3qkm3V1lm/VNyoJ0xCFFh7gEctkunU1aTTbh1aOdtRvNRwThQw==" });
        }
    }
}
