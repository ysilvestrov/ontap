using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ontap.Migrations
{
    public partial class MoreTaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeerServedInPubs");

            migrationBuilder.CreateTable(
                name: "BeerPrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    BeerId = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PubId = table.Column<string>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true),
                    Volume = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeerPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeerPrices_Beers_BeerId",
                        column: x => x.BeerId,
                        principalTable: "Beers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeerPrices_Pubs_PubId",
                        column: x => x.PubId,
                        principalTable: "Pubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kegs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    EmptyWeight = table.Column<decimal>(nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    Fitting = table.Column<char>(nullable: false),
                    IsReturnable = table.Column<bool>(nullable: false),
                    Material = table.Column<string>(nullable: true),
                    Volume = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kegs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Taps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Fitting = table.Column<char>(nullable: false),
                    HasHopinator = table.Column<bool>(nullable: false),
                    NitrogenPercentage = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    PubId = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taps_Pubs_PubId",
                        column: x => x.PubId,
                        principalTable: "Pubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeerKegs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    ArrivalDate = table.Column<DateTime>(nullable: false),
                    BeerId = table.Column<string>(nullable: false),
                    BestBeforeDate = table.Column<DateTime>(nullable: false),
                    BrewingDate = table.Column<DateTime>(nullable: false),
                    BuyerId = table.Column<string>(nullable: true),
                    DeinstallationDate = table.Column<DateTime>(nullable: false),
                    InstallationDate = table.Column<DateTime>(nullable: false),
                    KegId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeerKegs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeerKegs_Beers_BeerId",
                        column: x => x.BeerId,
                        principalTable: "Beers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeerKegs_Pubs_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Pubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeerKegs_Kegs_KegId",
                        column: x => x.KegId,
                        principalTable: "Kegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeerKegs_Breweries_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Breweries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeerKegsOnTap",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    DeinstallTime = table.Column<DateTime>(nullable: true),
                    InstallTime = table.Column<DateTime>(nullable: true),
                    KegId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    TapId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeerKegsOnTap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeerKegsOnTap_BeerKegs_KegId",
                        column: x => x.KegId,
                        principalTable: "BeerKegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeerKegsOnTap_Taps_TapId",
                        column: x => x.TapId,
                        principalTable: "Taps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeerKegWeights",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Date = table.Column<DateTime>(nullable: false),
                    KegId = table.Column<int>(nullable: false),
                    Weight = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeerKegWeights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeerKegWeights_BeerKegs_KegId",
                        column: x => x.KegId,
                        principalTable: "BeerKegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<bool>(
                name: "HasOwnBeers",
                table: "Breweries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BeerKegs_BeerId",
                table: "BeerKegs",
                column: "BeerId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerKegs_BuyerId",
                table: "BeerKegs",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerKegs_KegId",
                table: "BeerKegs",
                column: "KegId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerKegs_OwnerId",
                table: "BeerKegs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerKegsOnTap_KegId",
                table: "BeerKegsOnTap",
                column: "KegId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerKegsOnTap_TapId",
                table: "BeerKegsOnTap",
                column: "TapId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerKegWeights_KegId",
                table: "BeerKegWeights",
                column: "KegId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerPrices_BeerId",
                table: "BeerPrices",
                column: "BeerId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerPrices_PubId",
                table: "BeerPrices",
                column: "PubId");

            migrationBuilder.CreateIndex(
                name: "IX_Taps_PubId",
                table: "Taps",
                column: "PubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasOwnBeers",
                table: "Breweries");

            migrationBuilder.DropTable(
                name: "BeerKegsOnTap");

            migrationBuilder.DropTable(
                name: "BeerKegWeights");

            migrationBuilder.DropTable(
                name: "BeerPrices");

            migrationBuilder.DropTable(
                name: "Taps");

            migrationBuilder.DropTable(
                name: "BeerKegs");

            migrationBuilder.DropTable(
                name: "Kegs");

            migrationBuilder.CreateTable(
                name: "BeerServedInPubs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Price = table.Column<decimal>(nullable: false),
                    ServedId = table.Column<string>(nullable: true),
                    ServedInId = table.Column<string>(nullable: true),
                    Tap = table.Column<int>(nullable: false, defaultValue: 1)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Updated = table.Column<DateTime>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false, defaultValue: 0.5m)
                        .Annotation("MySql:ValueGeneratedOnAdd", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeerServedInPubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeerServedInPubs_Beers_ServedId",
                        column: x => x.ServedId,
                        principalTable: "Beers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeerServedInPubs_Pubs_ServedInId",
                        column: x => x.ServedInId,
                        principalTable: "Pubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeerServedInPubs_ServedId",
                table: "BeerServedInPubs",
                column: "ServedId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerServedInPubs_ServedInId",
                table: "BeerServedInPubs",
                column: "ServedInId");
        }
    }
}
