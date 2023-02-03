﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tasks4U.Models;

#nullable disable

namespace Tasks4U.Migrations
{
    [DbContext(typeof(TasksContext))]
    [Migration("20230202130631_AddedNotificationDates")]
    partial class AddedNotificationDates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("Tasks4U.Models.Task", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Desk")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("FinalDate")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("IntermediateDate")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("LastFinalNotificationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("LastIntermediateNotificationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RelatedTo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TaskFrequency")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}