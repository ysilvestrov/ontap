using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ontap.Migrations
{
    public partial class MorePubUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingUrl",
                table: "Pubs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "Pubs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VkontakteUrl",
                table: "Pubs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Pubs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingUrl",
                table: "Pubs");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "Pubs");

            migrationBuilder.DropColumn(
                name: "VkontakteUrl",
                table: "Pubs");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Pubs");
        }
    }
}
