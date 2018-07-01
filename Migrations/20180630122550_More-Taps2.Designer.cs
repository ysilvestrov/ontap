using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Ontap.Models;

namespace Ontap.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20180630122550_More-Taps2")]
    partial class MoreTaps2
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

            modelBuilder.Entity("Ontap.Models.BeerKeg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ArrivalDate");

                    b.Property<string>("BeerId")
                        .IsRequired();

                    b.Property<DateTime>("BestBeforeDate");

                    b.Property<DateTime>("BrewingDate");

                    b.Property<string>("BuyerId");

                    b.Property<DateTime>("DeinstallationDate");

                    b.Property<DateTime>("InstallationDate");

                    b.Property<int?>("KegId")
                        .IsRequired();

                    b.Property<string>("OwnerId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.HasIndex("BuyerId");

                    b.HasIndex("KegId");

                    b.HasIndex("OwnerId");

                    b.ToTable("BeerKegs");
                });

            modelBuilder.Entity("Ontap.Models.BeerKegOnTap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DeinstallTime");

                    b.Property<DateTime?>("InstallTime");

                    b.Property<int?>("KegId")
                        .IsRequired();

                    b.Property<int>("Priority");

                    b.Property<int?>("TapId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("KegId");

                    b.HasIndex("TapId");

                    b.ToTable("BeerKegsOnTap");
                });

            modelBuilder.Entity("Ontap.Models.BeerKegWeight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int?>("KegId")
                        .IsRequired();

                    b.Property<decimal>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("KegId");

                    b.ToTable("BeerKegWeights");
                });

            modelBuilder.Entity("Ontap.Models.BeerPrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BeerId")
                        .IsRequired();

                    b.Property<decimal>("Price");

                    b.Property<string>("PubId")
                        .IsRequired();

                    b.Property<DateTime>("Updated");

                    b.Property<DateTime>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<decimal>("Volume");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.HasIndex("PubId");

                    b.ToTable("BeerPrices");
                });

            modelBuilder.Entity("Ontap.Models.BeerSubstitution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BeerId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.ToTable("BeerSubstitutions");
                });

            modelBuilder.Entity("Ontap.Models.Brewery", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("CountryId");

                    b.Property<bool>("HasOwnBeers");

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

            modelBuilder.Entity("Ontap.Models.Keg", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("EmptyWeight");

                    b.Property<string>("ExternalId");

                    b.Property<char>("Fitting");

                    b.Property<bool>("IsReturnable");

                    b.Property<string>("Material");

                    b.Property<decimal>("Volume");

                    b.HasKey("Id");

                    b.ToTable("Kegs");
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

            modelBuilder.Entity("Ontap.Models.Tap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Fitting");

                    b.Property<bool>("HasHopinator");

                    b.Property<int>("NitrogenPercentage");

                    b.Property<string>("Number");

                    b.Property<string>("PubId")
                        .IsRequired();

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("PubId");

                    b.ToTable("Taps");
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

            modelBuilder.Entity("Ontap.Models.BeerKeg", b =>
                {
                    b.HasOne("Ontap.Models.Beer", "Beer")
                        .WithMany("BeerKegs")
                        .HasForeignKey("BeerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.Pub", "Buyer")
                        .WithMany("BeerKegsBought")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.Keg", "Keg")
                        .WithMany("BeerKegs")
                        .HasForeignKey("KegId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.Brewery", "Owner")
                        .WithMany("BeerKegsOwned")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ontap.Models.BeerKegOnTap", b =>
                {
                    b.HasOne("Ontap.Models.BeerKeg", "Keg")
                        .WithMany("BeerKegsOnTap")
                        .HasForeignKey("KegId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.Tap", "Tap")
                        .WithMany("BeerKegsOnTap")
                        .HasForeignKey("TapId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ontap.Models.BeerKegWeight", b =>
                {
                    b.HasOne("Ontap.Models.BeerKeg", "Keg")
                        .WithMany("Weights")
                        .HasForeignKey("KegId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ontap.Models.BeerPrice", b =>
                {
                    b.HasOne("Ontap.Models.Beer", "Beer")
                        .WithMany("BeerPrices")
                        .HasForeignKey("BeerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Ontap.Models.Pub", "Pub")
                        .WithMany("BeerPrices")
                        .HasForeignKey("PubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Ontap.Models.BeerSubstitution", b =>
                {
                    b.HasOne("Ontap.Models.Beer", "Beer")
                        .WithMany("Substitutions")
                        .HasForeignKey("BeerId")
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

            modelBuilder.Entity("Ontap.Models.Tap", b =>
                {
                    b.HasOne("Ontap.Models.Pub", "Pub")
                        .WithMany("Taps")
                        .HasForeignKey("PubId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
