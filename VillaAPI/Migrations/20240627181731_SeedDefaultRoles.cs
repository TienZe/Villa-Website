using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VillaAPI.Migrations
{
    public partial class SeedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7536ec76-5a36-402e-9a64-8517f6acce46", "b81c2e88-1c2c-4544-b686-cc47a2d40abd", "Admin", "ADMIN" },
                    { "fee2429e-d694-467c-9183-6c2a127d9a99", "4f9b24e1-bd37-4cb5-8598-3f928ac0bfad", "User", "USER" }
                });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 1, 17, 31, 373, DateTimeKind.Local).AddTicks(8476));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 1, 17, 31, 373, DateTimeKind.Local).AddTicks(8488));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 1, 17, 31, 373, DateTimeKind.Local).AddTicks(8489));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 1, 17, 31, 373, DateTimeKind.Local).AddTicks(8491));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 1, 17, 31, 373, DateTimeKind.Local).AddTicks(8492));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7536ec76-5a36-402e-9a64-8517f6acce46");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fee2429e-d694-467c-9183-6c2a127d9a99");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 0, 54, 42, 417, DateTimeKind.Local).AddTicks(7004));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 0, 54, 42, 417, DateTimeKind.Local).AddTicks(7015));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 0, 54, 42, 417, DateTimeKind.Local).AddTicks(7017));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 0, 54, 42, 417, DateTimeKind.Local).AddTicks(7018));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 28, 0, 54, 42, 417, DateTimeKind.Local).AddTicks(7020));
        }
    }
}
