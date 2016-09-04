﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Ontap.Models;

namespace ontap.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20160827081451_PricedBeer")]
    partial class PricedBeer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Ontap.Models.Beer", b =>
                {
                    b.Property<string>("Id");

                    b.Property<decimal>("Alcohol");

                    b.Property<string>("Brewery");

                    b.Property<string>("Description");

                    b.Property<decimal>("Gravity");

                    b.Property<decimal>("Ibu");

                    b.Property<int>("Kind");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Type");

                    b.Property<string>("_labels");

                    b.HasKey("Id");

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

            modelBuilder.Entity("Ontap.Models.City", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Cities");
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

            modelBuilder.Entity("Ontap.Models.Pub", b =>
                {
                    b.HasOne("Ontap.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");
                });
        }
    }
}
