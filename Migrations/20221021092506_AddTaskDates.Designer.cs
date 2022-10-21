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
    [Migration("20221021092506_AddTaskDates")]
    partial class AddTaskDates
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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Tasks4U.Models.TaskDate", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("Frequency")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsFinal")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TaskID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("TaskID");

                    b.ToTable("TaskDate");
                });

            modelBuilder.Entity("Tasks4U.Models.TaskDate", b =>
                {
                    b.HasOne("Tasks4U.Models.Task", null)
                        .WithMany("TaskDates")
                        .HasForeignKey("TaskID");
                });

            modelBuilder.Entity("Tasks4U.Models.Task", b =>
                {
                    b.Navigation("TaskDates");
                });
#pragma warning restore 612, 618
        }
    }
}
