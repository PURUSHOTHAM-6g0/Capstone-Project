﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManagement.Data;

#nullable disable

namespace TaskManagement.Migrations
{
    [DbContext(typeof(TaskDbcontext))]
    partial class TaskDbcontextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TaskManagement.Models.Employee", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EmployeeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EmployeeId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("TaskManagement.Models.EmployeeProject", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.HasKey("EmployeeId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TaskId");

                    b.ToTable("EmployeeProjects");
                });

            modelBuilder.Entity("TaskManagement.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectId"));

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProjectId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("TaskManagement.Models.Tasks", b =>
                {
                    b.Property<int>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskId"));

                    b.Property<string>("AssignedTo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TaskDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaskId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("TaskManagement.Models.EmployeeProject", b =>
                {
                    b.HasOne("TaskManagement.Models.Employee", "Employee")
                        .WithMany("EmployeeProjects")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskManagement.Models.Project", "Project")
                        .WithMany("EmployeeProjects")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskManagement.Models.Tasks", "Task")
                        .WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Employee");

                    b.Navigation("Project");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("TaskManagement.Models.Tasks", b =>
                {
                    b.HasOne("TaskManagement.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaskManagement.Models.Project", "Project")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("TaskManagement.Models.Employee", b =>
                {
                    b.Navigation("EmployeeProjects");
                });

            modelBuilder.Entity("TaskManagement.Models.Project", b =>
                {
                    b.Navigation("EmployeeProjects");

                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
