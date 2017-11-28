using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Ontap.Models;

namespace Ontap.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20171127215015_BJCP")]
    partial class BJCP
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Ontap.Models.Beer", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Alcohol");

                    b.Property<string>("BjcpStyle");

                    b.Property<string>("BreweryId");

                    b.Property<string>("Description");

                    b.Property<decimal>("Gravity");

                    b.Property<decimal>("Ibu");

                    b.Property<string>("Image");

                    b.Property<int>("Kind");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("ServeKind")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Type");

                    b.Property<string>("_labels");

                    b.HasKey("Id");

                    b.HasIndex("BreweryId");

                    b.ToTable("Beers");
                });

            modelBuilder.Entity("Ontap.Models.BeerServedInPubs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Price");

                    b.Property<string>("ServedId");

                    b.Property<string>("ServedInId");

                    b.Property<int>("Tap")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<DateTime>("Updated");

                    b.Property<decimal>("Volume")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0.5m);

                    b.HasKey("Id");

                    b.HasIndex("ServedId");

                    b.HasIndex("ServedInId");

                    b.ToTable("BeerServedInPubs");
                });

            modelBuilder.Entity("Ontap.Models.Brewery", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("CountryId");

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Breweries");
                });

            modelBuilder.Entity("Ontap.Models.BreweryAdmin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BreweryId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BreweryId");

                    b.HasIndex("UserId");

                    b.ToTable("BreweryAdmins");
                });

            modelBuilder.Entity("Ontap.Models.BrewerySubstitution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BreweryId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("BreweryId");

                    b.ToTable("BrewerySubstitutions");
                });

            modelBuilder.Entity("Ontap.Models.City", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Ontap.Models.Country", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Ontap.Models.Pub", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("BookingUrl");

                    b.Property<string>("CityId");

                    b.Property<string>("FacebookUrl");

                    b.Property<string>("Image");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ParserOptions");

                    b.Property<int>("TapNumber");

                    b.Property<string>("TaplistFooterImage");

                    b.Property<string>("TaplistHeaderImage");

                    b.Property<string>("VkontakteUrl");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Pubs");
                });

            modelBuilder.Entity("Ontap.Models.PubAdmin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PubId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PubId");

                    b.HasIndex("UserId");

                    b.ToTable("PubAdmins");
                });

            modelBuilder.Entity("Ontap.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanAdminBrewery");

                    b.Property<bool>("CanAdminPub");

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Ontap.Models.Beer", b =>
                {
                    b.HasOne("Ontap.Models.Brewery", "Brewery")
                        .WithMany("Beers")
                        .HasForeignKey("BreweryId");
                });

            modelBuilder.Entity("Ontap.Models.BeerServedInPubs", b =>
                {
                    b.HasOne("Ontap.Models.Beer", "Served")
                        .WithMany("BeerServedInPubs")
                        .HasForeignKey("ServedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.Pub", "ServedIn")
                        .WithMany("BeerServedInPubs")
                        .HasForeignKey("ServedInId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ontap.Models.Brewery", b =>
                {
                    b.HasOne("Ontap.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId");
                });

            modelBuilder.Entity("Ontap.Models.BreweryAdmin", b =>
                {
                    b.HasOne("Ontap.Models.Brewery", "Brewery")
                        .WithMany("Admins")
                        .HasForeignKey("BreweryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.User", "User")
                        .WithMany("BreweryAdmins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ontap.Models.BrewerySubstitution", b =>
                {
                    b.HasOne("Ontap.Models.Brewery", "Brewery")
                        .WithMany("Substitutions")
                        .HasForeignKey("BreweryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ontap.Models.Pub", b =>
                {
                    b.HasOne("Ontap.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");
                });

            modelBuilder.Entity("Ontap.Models.PubAdmin", b =>
                {
                    b.HasOne("Ontap.Models.Pub", "Pub")
                        .WithMany("Admins")
                        .HasForeignKey("PubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.User", "User")
                        .WithMany("PubAdmins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
