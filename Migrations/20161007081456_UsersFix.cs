using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ontap.Migrations
{
    public partial class UsersFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAdminPubBrewery",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "CanAdminBrewery",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAdminBrewery",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "CanAdminPubBrewery",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }
    }
}
