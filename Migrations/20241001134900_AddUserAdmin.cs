using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OloEcomm.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { "78011ea4-0208-41d2-82f8-daaa830587cc", "admin@example.com", "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEKpB5xqUaPhdg8EQQE2US5Wdr1E27vXEQkG8tkkAxBFZdBbXa32F3KVpx0JGefJkrQ==", "133-476-7890", "admin@example.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { "5d8f58df-5c12-49db-9967-f7e80da255ed", "dejlof@example.com", "DEJLOF@EXAMPLE.COM", "DEJLOF@EXAMPLE.COM", "AQAAAAIAAYagAAAAEDvPdw11ZUC7ZZEwgXpmj8dKlGrnD36UAgAQ0miwf++pAxho/mFXAL0SctS1w9tw/g==", "123-456-7890", "dejlof@example.com" });
        }
    }
}
