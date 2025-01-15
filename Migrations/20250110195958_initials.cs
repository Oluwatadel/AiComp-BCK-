using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiComp.Migrations
{
    /// <inheritdoc />
    public partial class initials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("92b0fba4-ec65-46ba-a8d5-414c84f20800"));

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MoodMesages",
                newName: "MoodMessageId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("1be4df47-06a8-4c66-98a7-67a5e3f3d821"), "admin@aicomp.com", false, "$2a$11$VZQZzoJ4itJ7IzzyIaXvz.n2N58IfKDPvWsJKIPEsTRozviaXizlu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1be4df47-06a8-4c66-98a7-67a5e3f3d821"));

            migrationBuilder.RenameColumn(
                name: "MoodMessageId",
                table: "MoodMesages",
                newName: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("92b0fba4-ec65-46ba-a8d5-414c84f20800"), "admin@aicomp.com", false, "$2a$11$CVgYknRQRg9Jj1it1wP9puUC9u38go42l9TxSHIyn1TqBQGa6OOrS" });
        }
    }
}
