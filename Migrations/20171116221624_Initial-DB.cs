using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ontap.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CanAdminBrewery = table.Column<bool>(nullable: false),
                    CanAdminPub = table.Column<bool>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pubs",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Address = table.Column<string>(nullable: true),
                    BookingUrl = table.Column<string>(nullable: true),
                    CityId = table.Column<string>(nullable: true),
                    FacebookUrl = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ParserOptions = table.Column<string>(nullable: true),
                    TapNumber = table.Column<int>(nullable: false),
                    VkontakteUrl = table.Column<string>(nullable: true),
                    WebsiteUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pubs_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Breweries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Address = table.Column<string>(nullable: true),
                    CountryId = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breweries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Breweries_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PubAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    PubId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PubAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PubAdmins_Pubs_PubId",
                        column: x => x.PubId,
                        principalTable: "Pubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PubAdmins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Alcohol = table.Column<decimal>(nullable: false),
                    BreweryId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Gravity = table.Column<decimal>(nullable: false),
                    Ibu = table.Column<decimal>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Kind = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ServeKind = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Type = table.Column<string>(nullable: true),
                    _labels = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beers_Breweries_BreweryId",
                        column: x => x.BreweryId,
                        principalTable: "Breweries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BreweryAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    BreweryId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreweryAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreweryAdmins_Breweries_BreweryId",
                        column: x => x.BreweryId,
                        principalTable: "Breweries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BreweryAdmins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrewerySubstitutions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    BreweryId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrewerySubstitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrewerySubstitutions_Breweries_BreweryId",
                        column: x => x.BreweryId,
                        principalTable: "Breweries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Beers_BreweryId",
                table: "Beers",
                column: "BreweryId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerServedInPubs_ServedId",
                table: "BeerServedInPubs",
                column: "ServedId");

            migrationBuilder.CreateIndex(
                name: "IX_BeerServedInPubs_ServedInId",
                table: "BeerServedInPubs",
                column: "ServedInId");

            migrationBuilder.CreateIndex(
                name: "IX_Breweries_CountryId",
                table: "Breweries",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_BreweryAdmins_BreweryId",
                table: "BreweryAdmins",
                column: "BreweryId");

            migrationBuilder.CreateIndex(
                name: "IX_BreweryAdmins_UserId",
                table: "BreweryAdmins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BrewerySubstitutions_BreweryId",
                table: "BrewerySubstitutions",
                column: "BreweryId");

            migrationBuilder.CreateIndex(
                name: "IX_Pubs_CityId",
                table: "Pubs",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_PubAdmins_PubId",
                table: "PubAdmins",
                column: "PubId");

            migrationBuilder.CreateIndex(
                name: "IX_PubAdmins_UserId",
                table: "PubAdmins",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeerServedInPubs");

            migrationBuilder.DropTable(
                name: "BreweryAdmins");

            migrationBuilder.DropTable(
                name: "BrewerySubstitutions");

            migrationBuilder.DropTable(
                name: "PubAdmins");

            migrationBuilder.DropTable(
                name: "Beers");

            migrationBuilder.DropTable(
                name: "Pubs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Breweries");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
