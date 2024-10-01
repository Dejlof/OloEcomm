using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OloEcomm.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5d8f58df-5c12-49db-9967-f7e80da255ed", "AQAAAAIAAYagAAAAEDvPdw11ZUC7ZZEwgXpmj8dKlGrnD36UAgAQ0miwf++pAxho/mFXAL0SctS1w9tw/g==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2479b9f3-1cb3-47be-8ce9-67e55a08ebd9", "AQAAAAIAAYagAAAAEJaTuk/jv1os005I25ermOPpoVUpOAEy5f2DSRtzh7m9zv0aEnpNYyfx8iquEq1Qzw==" });
        }
    }
}
