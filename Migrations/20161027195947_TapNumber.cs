using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ontap.Migrations
{
    public partial class TapNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreweryAdmins_Breweries_BreweryId",
                table: "BreweryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_BreweryAdmins_Users_UserId",
                table: "BreweryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_PubAdmins_Pubs_PubId",
                table: "PubAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_PubAdmins_Users_UserId",
                table: "PubAdmins");

            migrationBuilder.AddColumn<int>(
                name: "Tap",
                table: "BeerServedInPubs",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_BreweryAdmins_Breweries_BreweryId",
                table: "BreweryAdmins",
                column: "BreweryId",
                principalTable: "Breweries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BreweryAdmins_Users_UserId",
                table: "BreweryAdmins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PubAdmins_Pubs_PubId",
                table: "PubAdmins",
                column: "PubId",
                principalTable: "Pubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PubAdmins_Users_UserId",
                table: "PubAdmins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreweryAdmins_Breweries_BreweryId",
                table: "BreweryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_BreweryAdmins_Users_UserId",
                table: "BreweryAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_PubAdmins_Pubs_PubId",
                table: "PubAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_PubAdmins_Users_UserId",
                table: "PubAdmins");

            migrationBuilder.DropColumn(
                name: "Tap",
                table: "BeerServedInPubs");

            migrationBuilder.AddForeignKey(
                name: "FK_BreweryAdmins_Breweries_BreweryId",
                table: "BreweryAdmins",
                column: "BreweryId",
                principalTable: "Breweries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BreweryAdmins_Users_UserId",
                table: "BreweryAdmins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PubAdmins_Pubs_PubId",
                table: "PubAdmins",
                column: "PubId",
                principalTable: "Pubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PubAdmins_Users_UserId",
                table: "PubAdmins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
