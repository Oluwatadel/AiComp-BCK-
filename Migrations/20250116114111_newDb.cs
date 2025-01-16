using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiComp.Migrations
{
    /// <inheritdoc />
    public partial class newDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b1d3aaed-ad9c-4536-9a3b-8a1481fd116e"));

            migrationBuilder.CreateTable(
                name: "Journal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    TimeCreate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Journal_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("d67ed606-6153-47b9-be69-dcf010d5b483"), "admin@aicomp.com", false, "$2a$11$KTIbWd4nS2Y2u/wVFXJ4QeqnECqfOQVo3EoLyue0LxQO1zLck8HP2" });

            migrationBuilder.CreateIndex(
                name: "IX_Journal_UserId",
                table: "Journal",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Journal");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d67ed606-6153-47b9-be69-dcf010d5b483"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("b1d3aaed-ad9c-4536-9a3b-8a1481fd116e"), "admin@aicomp.com", false, "$2a$11$hox8kGiG2yH8vZ0D8PGeouxpxz1zweVa6KOLdIGZupWHz3pBDNcpy" });
        }
    }
}
