﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using cs_web_voting.Data;

#nullable disable

namespace cs_web_voting.Migrations
{
    [DbContext(typeof(CsWebVotingDbContext))]
    [Migration("20231214125733_InitialCsWebVotingDB")]
    partial class InitialCsWebVotingDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("cs_web_voting.Models.Maps", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("longtext");

                    b.Property<string>("type")
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("Maps");
                });
#pragma warning restore 612, 618
        }
    }
}
