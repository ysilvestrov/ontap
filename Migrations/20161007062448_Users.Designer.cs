using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Ontap.Models;

namespace ontap.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20161007062448_Users")]
    partial class Users
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Ontap.Models.Beer", b =>
                {
                    b.Property<string>("Id");

                    b.Property<decimal>("Alcohol");

                    b.Property<string>("BreweryId");

                    b.Property<string>("Description");

                    b.Property<decimal>("Gravity");

                    b.Property<decimal>("Ibu");

                    b.Property<int>("Kind");

                    b.Property<string>("Name")
                        .IsRequired();

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

                    b.HasKey("Id");

                    b.HasIndex("ServedId");

                    b.HasIndex("ServedInId");

                    b.ToTable("BeerServedInPubs");
                });

            modelBuilder.Entity("Ontap.Models.Brewery", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Address");

                    b.Property<string>("CountryId");

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

            modelBuilder.Entity("Ontap.Models.City", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Ontap.Models.Country", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Ontap.Models.Pub", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Address");

                    b.Property<string>("CityId");

                    b.Property<string>("Name")
                        .IsRequired();

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
                    b.Property<string>("Id");

                    b.Property<bool>("CanAdminPub");

                    b.Property<bool>("CanAdminPubBrewery");

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
                        .WithMany()
                        .HasForeignKey("BreweryId");

                    b.HasOne("Ontap.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
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
                        .WithMany()
                        .HasForeignKey("PubId");

                    b.HasOne("Ontap.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
