using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiComp.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1be4df47-06a8-4c66-98a7-67a5e3f3d821"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("b1d3aaed-ad9c-4536-9a3b-8a1481fd116e"), "admin@aicomp.com", false, "$2a$11$hox8kGiG2yH8vZ0D8PGeouxpxz1zweVa6KOLdIGZupWHz3pBDNcpy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1d3aaed-ad9c-4536-9a3b-8a1481fd116e"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("1be4df47-06a8-4c66-98a7-67a5e3f3d821"), "admin@aicomp.com", false, "$2a$11$VZQZzoJ4itJ7IzzyIaXvz.n2N58IfKDPvWsJKIPEsTRozviaXizlu" });
        }
    }
}
