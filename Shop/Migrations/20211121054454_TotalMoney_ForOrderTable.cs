﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMDTShop.Migrations
{
    public partial class TotalMoney_ForOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalMoney",
                table: "Order",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginViewModel");

            migrationBuilder.DropTable(
                name: "RegisterViewModel");

            migrationBuilder.DropColumn(
                name: "TotalMoney",
                table: "Order");
        }
    }
}
