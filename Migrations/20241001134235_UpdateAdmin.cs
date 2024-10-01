using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OloEcomm.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2479b9f3-1cb3-47be-8ce9-67e55a08ebd9", "AQAAAAIAAYagAAAAEJaTuk/jv1os005I25ermOPpoVUpOAEy5f2DSRtzh7m9zv0aEnpNYyfx8iquEq1Qzw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0f6bf67f-6265-4151-9d4b-6569e1d4d286", "AQAAAAIAAYagAAAAEDEFEt35X3M707VPXUGFcfVV51PChp8u+gBj4b8zCmjXaOUhecC0A6GHBjMDrG8Bww==" });
        }
    }
}
