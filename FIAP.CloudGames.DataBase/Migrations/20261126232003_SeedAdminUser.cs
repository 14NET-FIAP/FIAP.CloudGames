using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIAP.CloudGames.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "TypeUser" },
                values: new object[] { new Guid("3f2c9a1e-7b4d-4e8a-9c61-2d5f0b8e7a13"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@fiapcloudgames.com", true, "Administrador", "6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3f2c9a1e-7b4d-4e8a-9c61-2d5f0b8e7a13"));
        }
    }
}
