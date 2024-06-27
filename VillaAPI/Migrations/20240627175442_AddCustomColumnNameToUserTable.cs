using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VillaAPI.Migrations
{
    public partial class AddCustomColumnNameToUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 27, 23, 6, 8, 112, DateTimeKind.Local).AddTicks(4646));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 27, 23, 6, 8, 112, DateTimeKind.Local).AddTicks(4661));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 27, 23, 6, 8, 112, DateTimeKind.Local).AddTicks(4662));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 27, 23, 6, 8, 112, DateTimeKind.Local).AddTicks(4664));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 6, 27, 23, 6, 8, 112, DateTimeKind.Local).AddTicks(4665));
        }
    }
}
