using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiComp.Migrations
{
    /// <inheritdoc />
    public partial class newDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journal_Users_UserId",
                table: "Journal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Journal",
                table: "Journal");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d67ed606-6153-47b9-be69-dcf010d5b483"));

            migrationBuilder.RenameTable(
                name: "Journal",
                newName: "Journals");

            migrationBuilder.RenameIndex(
                name: "IX_Journal_UserId",
                table: "Journals",
                newName: "IX_Journals_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Journals",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Journals",
                table: "Journals",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    TimeOfActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("aa3b4ae4-c0db-4283-9684-283665bc7747"), "admin@aicomp.com", false, "$2a$11$re3e/Znxdu70vAfpLY3uwO/hs57eGlcI8JM3mdMgzvHd.kjLBMCEq" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journals_Users_UserId",
                table: "Journals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journals_Users_UserId",
                table: "Journals");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Journals",
                table: "Journals");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aa3b4ae4-c0db-4283-9684-283665bc7747"));

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Journals");

            migrationBuilder.RenameTable(
                name: "Journals",
                newName: "Journal");

            migrationBuilder.RenameIndex(
                name: "IX_Journals_UserId",
                table: "Journal",
                newName: "IX_Journal_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Journal",
                table: "Journal",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsConsented", "Password" },
                values: new object[] { new Guid("d67ed606-6153-47b9-be69-dcf010d5b483"), "admin@aicomp.com", false, "$2a$11$KTIbWd4nS2Y2u/wVFXJ4QeqnECqfOQVo3EoLyue0LxQO1zLck8HP2" });

            migrationBuilder.AddForeignKey(
                name: "FK_Journal_Users_UserId",
                table: "Journal",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
