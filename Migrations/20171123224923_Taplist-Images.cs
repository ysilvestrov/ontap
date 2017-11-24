using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ontap.Migrations
{
    public partial class TaplistImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaplistFooterImage",
                table: "Pubs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaplistHeaderImage",
                table: "Pubs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaplistFooterImage",
                table: "Pubs");

            migrationBuilder.DropColumn(
                name: "TaplistHeaderImage",
                table: "Pubs");
        }
    }
}
