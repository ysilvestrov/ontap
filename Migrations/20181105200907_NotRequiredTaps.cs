using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ontap.Migrations
{
    public partial class NotRequiredTaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BeerKegsOnTap_Taps_TapId",
                table: "BeerKegsOnTap");

            migrationBuilder.AlterColumn<int>(
                name: "TapId",
                table: "BeerKegsOnTap",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_BeerKegsOnTap_Taps_TapId",
                table: "BeerKegsOnTap",
                column: "TapId",
                principalTable: "Taps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BeerKegsOnTap_Taps_TapId",
                table: "BeerKegsOnTap");

            migrationBuilder.AlterColumn<int>(
                name: "TapId",
                table: "BeerKegsOnTap",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BeerKegsOnTap_Taps_TapId",
                table: "BeerKegsOnTap",
                column: "TapId",
                principalTable: "Taps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
