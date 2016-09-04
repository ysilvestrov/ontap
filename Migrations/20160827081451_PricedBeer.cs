using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ontap.Migrations
{
    public partial class PricedBeer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "BeerServedInPubs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Ibu",
                table: "Beers",
                nullable: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Gravity",
                table: "Beers",
                nullable: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Alcohol",
                table: "Beers",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "BeerServedInPubs");

            migrationBuilder.AlterColumn<double>(
                name: "Ibu",
                table: "Beers",
                nullable: false);

            migrationBuilder.AlterColumn<double>(
                name: "Gravity",
                table: "Beers",
                nullable: false);

            migrationBuilder.AlterColumn<double>(
                name: "Alcohol",
                table: "Beers",
                nullable: false);
        }
    }
}
